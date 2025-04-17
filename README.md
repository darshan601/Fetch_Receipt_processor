# 📄 Fetch Receipt Processor

A solution for [Fetch’s Receipt Processor Challenge](https://github.com/fetch-rewards/receipt-processor-challenge/tree/main).

---

## 📚 Table of Contents

1. [About the Problem](#about-the-problem)  
2. [Steps to Run](#steps-to-run)  
3. [API Endpoints](#api-endpoints)  
4. [Architecture & Components](#architecture--components)  
5. [Room for Improvements](#room-for-improvements)  

---

## 🧩 About the Problem

The objective is to build a web service that processes retail receipts and calculates reward points based on specific rules.

The service exposes the following endpoints:

- `POST /receipts/process` — Accepts receipt data and returns a unique receipt ID.
- `GET /receipts/{id}/points` — Returns the reward points associated with the given receipt ID.

Points are awarded based on various receipt attributes like:

- Retailer name  
- Purchase date and time  
- Item descriptions  
- Total amount  

Data persistence is limited to in-memory storage during runtime — no external database is required.

---

## 🚀 Steps to Run

1. **Clone the Repository**

```bash
git clone https://github.com/darshan601/Fetch_Receipt_processor.git
cd Fetch_Receipt_processor
```

2. **Build the Docker Image**

```bash
docker-compose build --no-cache
```
3. **Start the Application**

```bash
docker-compose up -d
```

Once started, access the app via http://localhost:18080
This opens the Swagger UI to interact with all API endpoints.

👉 Alternatively, you can use Postman to test the APIs.

4. **Stop the Application**

```bash
docker-compose down
```

This command shuts down the service and removes all containers.


## 🔌 API Endpoints

### `POST /receipts/process`  
Accepts receipt data and returns a unique receipt ID.

#### 📥 Example Request:
```json
{
  "retailer": "Target",
  "purchaseDate": "2022-01-02",
  "purchaseTime": "13:13",
  "total": "1.25",
  "items": [
    { "shortDescription": "Pepsi - 12-oz", "price": "1.25" }
  ]
}
```

#### 📤 Example Response:
```json
{
  "id": "7fb1377b-b223-49d9-a31a-5a02701dd310"
}
```

### `GET /receipts/{id}/points`
Returns the number of points awarded for the given receipt ID.

### 📤 Example Response:
```json
{
  "points": 32
}
```

## 🏗 Architecture & Components

### Components Overview:

- **Receipts Controller**  
  Exposes APIs for handling `POST` and `GET` requests.

- **Validation Service**  
  Validates the incoming receipt data to ensure it meets required criteria.

- **Receipt Service**  
  Handles conversion between request/response models and internal entity models.

- **Receipt Storage**  
  In-memory storage using a `ConcurrentDictionary` for receipt objects and a hash dictionary for duplicate detection.

- **Points Service**  
  Contains the logic for calculating points based on receipt attributes like item descriptions, time of purchase, and total amount.

- **Channel/Queue**  
  An unbounded channel acting as a message broker — receipt IDs are sent into the queue for asynchronous processing.

- **Receipt Processor (Background Worker)**  
  Consumes receipt IDs from the channel, processes them via the Points Service, and updates the stored receipt data.

