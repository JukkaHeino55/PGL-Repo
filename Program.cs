using System;
using System.IO;
using System.Linq;
using System.Text.Json;


    var builder = WebApplication.CreateBuilder(args);
    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

  // CORS asetukset. Mistä palvelua saa kutsua.
  // null = html (+ css ja js) file systemissä ilman että html tulee webappilta.
  // Tarkastettava tarvitseeko tuotannossa jos client on blobissa ilman 
  // appserviceä.
  // Jos ei tarvita nullia tuotannossa, niin nämä osoitteet appsettingsiin.

  builder.Services.AddCors(options => {
    options.AddPolicy(
      name: MyAllowSpecificOrigins,
      policy => {
        policy.WithOrigins(
          "http://localhost",
          "http://www.poolgaragelappee.fi",
          "null"
        )
        .WithMethods("PUT", "GET");;
      }
    );
  });

  var app = builder.Build();

  app.MapGet("/", () =>
  {
    Console.WriteLine("BBBBBB");
    return "Hello from PGLTalliVahti";
    // ToDo: API on toiminnassa + json - kellonaika - versio - started at
  });

app.MapPut("/Tapahtuma", async (HttpContext context) =>
{
using var reader = new StreamReader(context.Request.Body);
var requestBody = await reader.ReadToEndAsync(); // Read the request body
Console.WriteLine($"Received request: {requestBody}");
MyNamespace.MyClass.MyProperty += "Tapahtuma";
Console.WriteLine(MyNamespace.MyClass.MyProperty);
Console.WriteLine($"Received request: {requestBody}");

string states = JsonDocument.Parse(requestBody).RootElement.GetProperty("states").GetString();


  string content = File.ReadAllText("states.txt");
  int index3 = -1;
  int index2 = 0;
string changedId = "" + JsonDocument.Parse(requestBody).RootElement.GetProperty("id").ToString();


  Console.WriteLine($"Tähän testataan: {changedId}");

  string jasen = "UNKNOWN";
  foreach (var entry in MyNamespace.MyClass.Jasenet)
  {
    if (entry[0] == changedId)
    {
      index3 = index2;
      jasen = entry[1];
      break;
    }
    index2++;
  }

  if (index3 != -1) {
    Console.WriteLine("Match");
  } else {
      Console.WriteLine("No Match");
    }

  string modified = content;
  string actionToDo = "X";
  if (index3 != -1)
  {
    actionToDo = "" + JsonDocument.Parse(requestBody).RootElement.GetProperty("operation").ToString();
    Console.WriteLine("Action to do = " + actionToDo);

    string tmpStr = actionToDo;
    if (tmpStr == "3") tmpStr = "0";
    modified = content.Remove(index3, 1).Insert(index3, tmpStr);
  }
  else
  {
    Console.WriteLine("NOT OK");
  }

  storeStates(modified);

  switch (actionToDo)
  {
    case "0":
      WriteLog("30", jasen + " poistui");
      break;
    case "1":
      WriteLog("31", jasen + " saapui");
      break;
    case "2":
      WriteLog("32", jasen + " on tulossa");
      break;
    case "3":
      WriteLog("33", jasen + " perui tulon");
      break;
    default:
      WriteLog("39", jasen + "Virhe !!!");
      break;
  }

  return Results.Ok("ok");
  });

app.MapGet("/refresh", () =>
{
    string content = File.ReadAllText("states.txt");
    return Results.Text(content);
});


app.MapGet("/Pelaajat", () =>
  {
    string json = "[";
    string comma = "";
    string paikallaStatus = File.ReadAllText("states.txt");

    int index = 0;

    foreach (var entry in MyNamespace.MyClass.Jasenet)
    {
      Console.WriteLine($"Line {entry[0]}: {entry[1]}");
      string paikalla = paikallaStatus.Substring(index,1);
      string record = $"{{\"id\": \"{entry[0]}\", \"nimi\": \"{entry[1]}\", \"paikalla\": \"{paikalla}\"}}";
      json += comma + record;
      comma = ",";
      index++;
    }
    json += "]";

    Console.WriteLine(json);
    Console.WriteLine("Pelaajat kutsuttu");

    return json;
  });

app.MapGet("/GetTapahtumat51Reverse", () =>
{
    string content = File.ReadAllText("log.txt");
    return Results.Text(content);
});

app.MapGet("/ListaaTapahtumat/{count:int}/{direction:int}", (int count, int direction) =>
{
    string[] lines = File.ReadAllLines("log.txt");

    IEnumerable<string> selected;

    if (direction == 1 || direction == -1)
    {
        selected = lines.Take(count);
    }
    else if (direction == 2 || direction == -2)
    {
        selected = lines.Reverse().Take(count);
    }
    else
    {
        return Results.BadRequest("Direction must be 1, 2, -1, or -2");
    }

    if (direction < 0)
    {
        selected = selected.Reverse(); // reverse the selected records
    }

    string result = string.Join("\n", selected);
    return Results.Text(result);
});



Console.WriteLine("Palvelu käynnistyi");
  WriteLog("01", "Palvelu käynnistyi");
  app.UseCors(MyAllowSpecificOrigins);
  MyNamespace.MyClass.MyProperty = "Started";
  Console.WriteLine(MyNamespace.MyClass.MyProperty);
  HaeJasenet();
  app.Run();
  MyNamespace.MyClass.MyProperty += "Ended";
  Console.WriteLine(MyNamespace.MyClass.MyProperty);
  WriteLog("02", "Palvelu lopetti");

void HaeJasenet()
{

  string filePath = "Jasenet.txt"; // Path to your file
  try
  {
    using (StreamReader reader = new StreamReader(filePath))
    {
  
      List<string[]> linesList = new List<string[]>(); // 2D array-like structure

      string? line;
      while ((line = reader.ReadLine()) != null) // Reads one line at a time
      {
        if (line.Length > 0 && line[0] != '#')
        {
          Console.WriteLine(line);
          string[] s = line.Split(';');
          linesList.Add(new string[] { s[0], s[1] });
          Console.WriteLine($"Xs1 = {s[0]}");
          Console.WriteLine($"Xs2 = {s[1]}");
        }
      }
      MyNamespace.MyClass.Jasenet = linesList;
    }
  }
  catch (Exception ex)
  {
    Console.WriteLine($"Error: {ex.Message}");
  }


}

void WriteLog(string tyyppi, string msg)
{
  // tyyppi
  // 01 = Ohjelman käynnistys
  // 02 = Ohjelman sammutus
  // 10 = Jäsenen tilan muutos
  var aika = DateTime.Now.ToString("yyyyMMddHHmmss"); // case sensitive;
    using var writer = new StreamWriter("log.txt", append: true);
      writer.WriteLine($"{tyyppi};{aika};{msg}");
  
}

void storeStates(string states)
{
    File.WriteAllText("states.txt", states);
}

namespace MyNamespace
{
    public class MyClass
    {
        public static string MyProperty { get; set; } = "Test";
        public static List<string[]> Jasenet { get; set; } = [];
        public static string MyProperty3 { get; set; } = "Test";

    }
}