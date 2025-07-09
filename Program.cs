using System.Text.Json;
using System.IO;


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
    return "Hello World!";
    // ToDo: API on toiminnassa + json - kellonaika - versio - started at
  });
    
  app.MapPut("/Tapahtuma", async (HttpContext context) =>
  {
      using var reader = new StreamReader(context.Request.Body);
      var requestBody = await reader.ReadToEndAsync(); // Read the request body
      Console.WriteLine($"Received request: {requestBody}");
      MyNamespace.MyClass.MyProperty += "Tapahtuma";
      Console.WriteLine(MyNamespace.MyClass.MyProperty);
      // write to file
    using var writer = new StreamWriter("log.txt", append: true);
      writer.WriteLine($"Received request: {requestBody}");

      //JH 20250707
      //---
//      using var doc = JsonDocument.Parse(requestBody);
    
      string states = JsonDocument.Parse(requestBody).RootElement.GetProperty("states").GetString();
      storeStates(states);


      // Set a variable to the Documents path.

      // Write the text to a new file named "WriteFile.txt".
      // Tämä tyhjentää tiedoston ennen kirjoitusta
      //File.WriteAllText(Path.Combine(".", "WriteFile.txt"), requestBody);

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
    foreach (var entry in MyNamespace.MyClass.Jasenet)
    {
      Console.WriteLine($"Line {entry[0]}: {entry[1]}");
      string record = $"{{\"id\": \"{entry[0]}\", \"nimi\": \"{entry[1]}\", \"paikalla\": \"0\"}}";
      json += comma + record;
      comma = ",";
    }
    json += "]";

    Console.WriteLine(json);
    Console.WriteLine("Pelaajat kutsuttu");

    WriteLog("10", "Pelaajat kutsuttu");

    return json;
  });

  Console.WriteLine("AAA");
  app.UseCors(MyAllowSpecificOrigins);
  MyNamespace.MyClass.MyProperty = "Started";
  Console.WriteLine(MyNamespace.MyClass.MyProperty);
  HaeJasenet();
  app.Run();
  MyNamespace.MyClass.MyProperty += "Ended";
  Console.WriteLine(MyNamespace.MyClass.MyProperty);
  WriteLog("02", "Program end");

  /*          foreach (var entry in MyNamespace.MyClass.Jasenet)
            {
                Console.WriteLine($"Line {entry[0]}: {entry[1]}");
            }
  */

void HaeJasenet()
{
  WriteLog("01", "Program start");

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