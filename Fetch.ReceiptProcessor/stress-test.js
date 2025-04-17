import http from 'k6/http';
import { check, sleep } from 'k6';

// Options for VUs and iterations
export let options = {
    vus: 5000,         // Virtual Users
    iterations: 100000, // Total iterations to run
};

// Helper functions to generate random data
function randomFloat(min, max) {
    return (Math.random() * (max - min) + min).toFixed(2);
}

function randomTime() {
    const hour = String(Math.floor(Math.random() * 24)).padStart(2, '0');
    const minute = String(Math.floor(Math.random() * 60)).padStart(2, '0');
    return `${hour}:${minute}`;
}

function randomRetailer() {
    const list = ['Target', 'Walmart', 'Costco', 'TraderJoes', 'BestBuy'];
    return list[Math.floor(Math.random() * list.length)];
}

// Array to store created receipt IDs
let createdReceiptIds = [];

// Flag to control when to start mixing GET and POST requests
let hasStartedMixing = false;

export default function () {
    const isPostRequest = Math.random() > 0.5; // 50% chance to send a POST request or GET request

    if (!hasStartedMixing) {
        // First, send 10,000 (or 20,000) POST requests
        if (createdReceiptIds.length < 20000) {
            // POST request to process a receipt
            const url = 'http://host.docker.internal:5232/receipts/process';

            const payload = JSON.stringify({
                retailer: randomRetailer(),
                purchaseDate: '2023-04-15',
                purchaseTime: randomTime(),
                total: randomFloat(10, 200),
                items: [
                    {
                        shortDescription: 'Item ' + Math.floor(Math.random() * 1000),
                        price: randomFloat(1, 50),
                    }
                ]
            });

            const params = {
                headers: {
                    'Content-Type': 'application/json',
                },
            };

            const res = http.post(url, payload, params);

            // Capture the receiptId from the POST response
            const jsonResponse = res.json();
            const receiptId = jsonResponse.id; // Assuming the response has 'id' field

            if (receiptId) {
                createdReceiptIds.push(receiptId); // Store the receiptId for future GET requests
            }

            check(res, {
                'POST status is 200': (r) => r.status === 200,
            });
        } else {
            // Once we've sent the initial 20,000 POST requests, start mixing POST and GET requests
            hasStartedMixing = true;
        }
    } else {
        // Once mixing starts, randomly decide whether to send a POST or GET request
        if (isPostRequest) {
            // POST request to process a receipt (if you want more POST requests to continue)
            const url = 'http://host.docker.internal:5232/receipts/process';

            const payload = JSON.stringify({
                retailer: randomRetailer(),
                purchaseDate: '2023-04-15',
                purchaseTime: randomTime(),
                total: randomFloat(10, 200),
                items: [
                    {
                        shortDescription: 'Item ' + Math.floor(Math.random() * 1000),
                        price: randomFloat(1, 50),
                    }
                ]
            });

            const params = {
                headers: {
                    'Content-Type': 'application/json',
                },
            };

            const res = http.post(url, payload, params);

            // Capture the receiptId from the POST response
            const jsonResponse = res.json();
            const receiptId = jsonResponse.id; // Assuming the response has 'id' field

            if (receiptId) {
                createdReceiptIds.push(receiptId); // Store the receiptId for future GET requests
            }

            check(res, {
                'POST status is 200': (r) => r.status === 200,
            });

        } else {
            // GET request to fetch points for a receipt (with a random receiptId from the list)
            if (createdReceiptIds.length > 0) {
                // Get a random receiptId from the list of created receipts
                const receiptId = createdReceiptIds[Math.floor(Math.random() * createdReceiptIds.length)];

                const url = `http://host.docker.internal:5232/receipts/${receiptId}/points`;

                const res = http.get(url);

                check(res, {
                    'GET status is 200': (r) => r.status === 200,
                });
            } else {
                // If no POST requests have been made yet, skip GET request
                console.log('Skipping GET request as no POST requests have been made yet.');
            }
        }
    }

    // Sleep to avoid overloading the system and simulate real-world users
    // sleep(0.5);  // Optional: Add a small delay between requests
}
