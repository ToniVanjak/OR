window.onload = authenticated();

async function getData() {
    const response = await fetch('/api/igraci');
    const data = await response.json();
    console.log(data);


    var tbody = document.querySelector('#data-table tbody');

    data.forEach(function (item) {

        var row = tbody.insertRow();

        for (var key in item) {
            var cell = row.insertCell();
            cell.appendChild(document.createTextNode(item[key]));
        }
    })
};

window.onload = getData;

document.getElementById('loginButton').addEventListener('click', login);
function login() {

    window.location.href = "/api/igraci/login";
     authenticated();
}

document.getElementById('logoutButton').addEventListener('click', logout);
function logout() {
    window.location.href = "/api/igraci/logout";
    authenticated();
}

document.getElementById('profileButton').addEventListener('click', profile);

document.getElementById('osvjeziButton').addEventListener('click', exportData);

async function exportData() {
    const response = await fetch('/api/igraci/extract');
    const data = await response.json();

    if (!data || data.length === 0) {
        console.error('No igraci data available for export');
        return;
    }

    const igraciCsvContent = data.map(row => Object.values(row).join(',')).join('\n');

    const igraciCsvBlob = new Blob([igraciCsvContent], { type: 'text/csv' });
    const igraciCsvUrl = URL.createObjectURL(igraciCsvBlob);
    const igraciCsvLink = document.createElement('a');
    igraciCsvLink.href = igraciCsvUrl;
    igraciCsvLink.download = 'igraci.csv';
    document.body.appendChild(igraciCsvLink);
    igraciCsvLink.click();
    document.body.removeChild(igraciCsvLink);

    const igraciJsonContent = JSON.stringify(data, null, 2);
    const igraciJsonBlob = new Blob([igraciJsonContent], { type: 'application/json' });
    const igraciJsonUrl = URL.createObjectURL(igraciJsonBlob);
    const igraciJsonLink = document.createElement('a');
    igraciJsonLink.href = igraciJsonUrl;
    igraciJsonLink.download = 'igraci.json';
    document.body.appendChild(igraciJsonLink);
    igraciJsonLink.click();
    document.body.removeChild(igraciJsonLink);

    const kluboviResponse = await fetch('/api/klubovi/extract');
    const kluboviData = await kluboviResponse.json();

    if (!kluboviData || kluboviData.length === 0) {
        console.error('No klubovi data available for export');
        return;
    }

    const csvContent = kluboviData.map(row => Object.values(row).join(',')).join('\n');

    const csvBlob = new Blob([csvContent], { type: 'text/csv' });
    const csvUrl = URL.createObjectURL(csvBlob);
    const csvLink = document.createElement('a');
    csvLink.href = csvUrl;
    csvLink.download = 'klubovi.csv';
    document.body.appendChild(csvLink);
    csvLink.click();
    document.body.removeChild(csvLink);

    const jsonContent = JSON.stringify(kluboviData, null, 2);
    const jsonBlob = new Blob([jsonContent], { type: 'application/json' });
    const jsonUrl = URL.createObjectURL(jsonBlob);
    const jsonLink = document.createElement('a');
    jsonLink.href = jsonUrl;
    jsonLink.download = 'klubovi.json';
    document.body.appendChild(jsonLink);
    jsonLink.click();
    document.body.removeChild(jsonLink);



}

function profile() {
    window.location.href = "/api/igraci/profile";
}

function authenticated() {

    fetch("/api/igraci/authenticated", {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
    })
        .then(response => response.json())
        .then(data => {
            if (data === false) {
                document.getElementById('logoutButton').style.display = 'none';
                document.getElementById('profileButton').style.display = 'none';
                document.getElementById('osvjeziButton').style.display = 'none';
            }
            else {
                document.getElementById('logoutButton').style.display = 'block';
                document.getElementById('profileButton').style.display = 'block';
                document.getElementById('osvjeziButton').style.display = 'block';
            }
        })
        .catch(error => {
            console.error('Error fetching authenticated:', error);
        });
}



const downloadButton = document.getElementById('csv');
downloadButton.addEventListener('click', function () {

    const filteredRows = dataTable.querySelectorAll("tbody tr");

    let csvContent = "data:text/csv;charset=utf-8,";

    filteredRows.forEach(function (row) {
        const rowData = Array.from(row.cells).map(cell => cell.innerText);
        if (row.style.display === '') {
            csvContent += rowData.join(",") + "\n";
        }
    });

    const encodedUri = encodeURI(csvContent);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "filter.csv");
    document.body.appendChild(link);

    link.click();
});

const downloadJsonButton = document.getElementById('json');
downloadJsonButton.addEventListener('click', function () {

    const filteredRows = dataTable.querySelectorAll('tbody tr');

    const jsonData = [];

    filteredRows.forEach(function (row) {
        const rowData = {};
        Array.from(row.cells).forEach((cell, index) => {
            const headerText = dataTable.querySelector('thead tr').cells[index].innerText;
            if (row.style.display === '') {
                rowData[headerText] = cell.innerText;
            }
        });
        if (row.style.display === '') {
            jsonData.push(rowData);
        }
    });

    const jsonString = JSON.stringify(jsonData, null, 2);

    const encodedUri = encodeURI(`data:text/json;charset=utf-8,${jsonString}`);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "filter.json");
    document.body.appendChild(link);

    link.click();
});



const categoryDropdown = document.getElementById('category');
const searchInput = document.getElementById('searchInput');
const dataTable = document.getElementById('data-table');

function filterTable() {
    const query = searchInput.value.toLowerCase();
    const category = categoryDropdown.value;
    const rows = dataTable.querySelectorAll('tbody tr');

    if (category == "Sva polja") {
        rows.forEach(row => {
            const textContent = row.textContent.toLowerCase();

            if (textContent.includes(query)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    } else {
        rows.forEach(row => {
            const cellContent = row.querySelector(`td:nth-child(${getColumnIndex(category)})`).textContent.toLowerCase();
            if (cellContent.includes(query)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    }
}

function getColumnIndex(category) {
    const headerRow = dataTable.querySelector('thead tr');
    const headers = Array.from(headerRow.children);
    return headers.findIndex(header => header.textContent.toLowerCase() === category.toLowerCase()) + 1;
}

categoryDropdown.addEventListener('change', filterTable);
searchInput.addEventListener('input', filterTable);

function changeRoute() {

    window.location.href = "datatable.html";
}