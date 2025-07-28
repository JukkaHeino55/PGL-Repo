//var serverport = "localhost:5148";
var serverport = "pgltallivahti02-d8fgd6a0apgmhnb0.swedencentral-01.azurewebsites.net"
function myFunc(id) {
  const queryString = window.location.search;
  const URLParams = new URLSearchParams(queryString);
  const rooli = URLParams.get("parm");

  const target = document.getElementById('item' + id);
  let operation; // 0 = poistui ; 1 = saapui ; 2 = ilmoittautui; 3 = perui ilmoittautumisen
  let states = "";

  if (target.style.backgroundColor === 'green' || target.style.backgroundColor === 'yellow') {
    if (target.style.backgroundColor === 'yellow' && rooli) {
      target.style.backgroundColor = 'green';
      operation = "1";
    } else {
      if (target.style.backgroundColor === 'yellow') {
        // alert('OPERATION 3');
        operation = "3";
      } else {
        operation = "0";
      }
      target.style.backgroundColor = 'gray';
    }
  } else {
    target.style.backgroundColor = rooli ? 'green' : 'yellow';
    operation = rooli ? "1" : "2";
  }

  const container = document.getElementById('container1');
  container.childNodes.forEach(node => {
    if (node.nodeType === Node.ELEMENT_NODE && node.id) {
      let tila = "";
      switch(node.style.backgroundColor) {
        case 'gray': tila = "0"; break;
        case 'green': tila = "1"; break;
        case 'yellow': tila = "2"; break;
        default: tila = "9";
      }
      states += tila;
    }
  });

  const params = {
    id,
    operation,
    states
  };

  const options = {
    method: 'PUT',
    body: JSON.stringify(params)
  };

  fetch('https://' + serverport + '/Tapahtuma', options)
    .then(response => response.json())
    .then(response => {
      init2();
    });
    
}

function addElement(container, id, link, paikalla) {
  const queryString = window.location.search;
  const URLParams = new URLSearchParams(queryString);
  const rooli = URLParams.get("parm");

  const newElement = document.createElement("div");
  newElement.id = "item" + id;
  newElement.className = "item content" + id;
  newElement.setAttribute("onclick", `myFunc(${id});`);

  const heading = document.createElement("h1");
  heading.innerHTML = link;
  newElement.appendChild(heading);
  container.appendChild(newElement);

  const target = document.getElementById('item' + id);
  let vari;
  switch(paikalla) {
    case "0": vari = 'gray'; break;
    case "1": vari = 'green'; break;
    case "2": vari = 'yellow'; break;
    default: vari = 'red';
  }
  target.style.backgroundColor = vari;
}


function updElement(container, id, link, paikalla) {
  const target = document.getElementById('item' + id);
  let vari;
  switch(paikalla) {
    case "0": vari = 'gray'; break;
    case "1": vari = 'green'; break;
    case "2": vari = 'yellow'; break;
    default: vari = 'red';
  }
  target.style.backgroundColor = vari;

}

function lokiPainettu() {
//  alert("Painettu!");
  const el1 = document.getElementById('container1');
  const el2 = document.getElementById('container2');

//  alert(el1.style.display);

  if (el1.style.display === "none") {
    el1.style.display = "";
    el2.style.display = "none";
  } else {
    var table = document.getElementById("taulu");
    el1.style.display = "none";
    el2.style.display = "";


    fetchRecords(table, 20, -2);
  }

}

function fetchRecords(table, count, direction) {

    // Clear old rows except header
  while (table.rows.length > 1) {
    table.deleteRow(1);
  }
  
  const url = 'https://' + serverport + `/ListaaTapahtumat/${count}/${direction}`;
  fetch(url)
    .then(response => {
      if (!response.ok) {
        throw new Error("Network response was not ok");
      }
      return response.text(); // we expect raw text
    })
    .then(data => {
      const lines = data.split('\n'); // split into lines
      lines.forEach((line, index) => {
        console.log(`Line ${index + 1}: ${line}`);
        var row = table.insertRow(1);
        var cell1 = row.insertCell(0);
        var cell2 = row.insertCell(1);
        var cell3 = row.insertCell(2);
        cell1.innerHTML = line.slice(9, 11) + "." + line.slice(7, 9) + "." + line.slice(3, 7);
        cell2.innerHTML = line.substring(11, 13) + ":" + line.substring(13, 15) + ":" + line.substring(15, 17);
        cell3.innerHTML = line.slice(18);
      });
    })
    .catch(error => {
      console.error("Fetch error:", error);
    });
}

async function RefreshPainettu() {
  const queryString = window.location.search;
  const URLParams = new URLSearchParams(queryString);
  const arvo = URLParams.get("parm");

  const response = await fetch('https://' + serverport + '/refresh');
  const text = await response.text();

  init();
}

function init() {
  const container = document.getElementById('container1');
  let pelaajat;
  const myNode = document.getElementById("container1");
  while (myNode.lastElementChild) {
    myNode.removeChild(myNode.lastElementChild);
  }

  fetch('https://' + serverport + '/Pelaajat')
    .then(response => response.json())
    .then(data => {
      pelaajat = data;
      pelaajat.forEach(pelaaja => {
        addElement(container, pelaaja.id, pelaaja.nimi, pelaaja.paikalla);
      });
    })
    .catch(error => alert('Error:', error));
}

function init2() {
  const container = document.getElementById('container1');
  let pelaajat;

  fetch('https://' + serverport + '/Pelaajat')
    .then(response => response.json())
    .then(data => {
      pelaajat = data;
      pelaajat.forEach(pelaaja => {
        updElement(container, pelaaja.id, pelaaja.nimi, pelaaja.paikalla);
      });
    })
    .catch(error => alert('Error:', error));
}

init();

