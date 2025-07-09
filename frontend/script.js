var serverport = "localhost:5148";

function myFunc(id) {
  const target = document.getElementById('item' + id);
  let operation; // 0 = poistui ; 1 = saapui
  let states = "";

  if (target.style.backgroundColor === 'green') {
    target.style.backgroundColor = 'gray';
    operation = "0";
  } else {
    target.style.backgroundColor = 'green';
    operation = "1";
  }

  const container = document.getElementById('container1');
  container.childNodes.forEach(node => {
    if (node.nodeType === Node.ELEMENT_NODE && node.id) {
      states += node.style.backgroundColor === 'gray' ? "0" : "1";
      // alert(node.id);
      // console.log(node.style.backgroundColor);
    }
  });

  const params = {
    id,
    operation,
    states
  };

  alert(JSON.stringify(params, null, 2));

  const options = {
    method: 'PUT',
    body: JSON.stringify(params)
  };

  fetch('http://' + serverport + '/Tapahtuma', options)
    .then(response => response.json())
    .then(response => {
      // Do something with response.
    });
}

function addElement(container, id, link, paikalla) {
  const newElement = document.createElement("div");
  newElement.id = "item" + id;
  newElement.className = "item content" + id;
  newElement.setAttribute("onclick", `myFunc(${id});`);

  const heading = document.createElement("h1");
  heading.innerHTML = link;
  newElement.appendChild(heading);
  container.appendChild(newElement);

  const target = document.getElementById('item' + id);
  target.style.backgroundColor = paikalla === "1" ? 'green' : 'gray';
}

function lokiPainettu() {
  alert("Painettu!");
  const el1 = document.getElementById('container1');
  const el2 = document.getElementById('container2');

  alert(el1.style.display);

  if (el1.style.display === "none") {
    el1.style.display = "";
    el2.style.display = "none";
  } else {
    el1.style.display = "none";
    el2.style.display = "";
  }
}

async function RefreshPainettu() {
  alert("Refresh painettu");

  const response = await fetch('http://' + serverport + '/refresh');
  const text = await response.text();
  alert(text);
}

function init() {
  const container = document.getElementById('container1');
  let pelaajat;

  fetch('http://' + serverport + '/Pelaajat')
    .then(response => response.json())
    .then(data => {
      pelaajat = data;
      pelaajat.forEach(pelaaja => {
        addElement(container, pelaaja.id, pelaaja.nimi, pelaaja.paikalla);
      });
    })
    .catch(error => alert('Error:', error));
}

init();

/*
ToDos:
- Henkilön ID:tä ei tarvita selaimella. Pelkkä järjestys riittää tunnistukseen.
  API tietää mikä on järjestyksessä n:nen henkilön id.
  Koska nämä ovat feikki-id:tä, niin olkoon nyt kuitenkin mukana pilotointivaiheessa.
*/
