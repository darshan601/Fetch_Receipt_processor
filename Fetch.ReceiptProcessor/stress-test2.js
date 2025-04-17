import http from 'k6/http';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';

// K6 Test Options
export let options = {
    stages: [
        { duration: '30s', target: 1000 },  // ramp-up
        { duration: '1m', target: 2500 },   // stay at 2500 users
        { duration: '1m', target: 5000 },   // ramp to 5000
        { duration: '2m', target: 5000 },   // hold
        { duration: '30s', target: 0 },     // ramp-down
    ],
    thresholds: {
        http_req_duration: ['p(95)<1000'], // 95% of requests should be < 1s
        http_req_failed: ['rate<0.01'],    // <1% request failures allowed
    },
};

// Utilities
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

// Shared ID pool
let receiptIds = [];

export default function () {
    const isPost = receiptIds.length < 20000 || Math.random() > 0.5;

    if (isPost) {
        // POST
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

        const res = http.post(url, payload, {
            headers: { 'Content-Type': 'application/json' },
        });

        const receiptId = res.json('id');
        if (receiptId) {
            // Prevent unlimited growth
            if (receiptIds.length < 50000) {
                receiptIds.push(receiptId);
            }
        }

        check(res, {
            'POST status is 200': (r) => r.status === 200,
        });

    } else {
        // GET
        if (receiptIds.length > 0) {
            const receiptId = receiptIds[Math.floor(Math.random() * receiptIds.length)];
            const url = `http://host.docker.internal:5232/receipts/${receiptId}/points`;

            const res = http.get(url);

            check(res, {
                'GET status is 200': (r) => r.status === 200,
            });

        } else {
            console.warn('GET skipped — no receipt IDs yet');
        }
    }

    // Optional: Delay to simulate user think time
    // sleep(0.1);
}

// docker run --rm -i grafana/k6 run - < stress-test2.js
// TOTAL RESULTS
//
// checks_total.......................: 333131  1109.981515/s
// checks_succeeded...................: 100.00% 333131 out of 333131
// checks_failed......................: 0.00%   0 out of 333131
//
//     ✓ POST status is 200
// HTTP
// http_req_duration.......................................................: avg=3.05s min=398.45µs med=2.61s max=12.53s p(90)=6.24s p(95)=6.93s
// { expected_response:true }............................................: avg=3.05s min=398.45µs med=2.61s max=12.53s p(90)=6.24s p(95)=6.93s
// http_req_failed.........................................................: 0.00%  0 out of 333131
// http_reqs...............................................................: 333131 1109.981515/s
//
// EXECUTION
// iteration_duration......................................................: avg=3.08s min=481.95µs med=2.64s max=12.52s p(90)=6.29s p(95)=6.97s
// iterations..............................................................: 333131 1109.981515/s
// vus.....................................................................: 94     min=17          max=5000
// vus_max.................................................................: 5000   min=5000        max=5000
//
// NETWORK
// data_received...........................................................: 68 MB  226 kB/s
// data_sent...............................................................: 104 MB 346 kB/s
//
//
//
// running (5m00.1s), 0000/5000 VUs, 333131 complete and 0 interrupted iterations
// default ✓ [ 100% ] 0000/5000 VUs  5m0s
// time="2025-04-17T08:34:41Z" level=error msg="thresholds on metrics 'http_req_duration' have been crossed"
