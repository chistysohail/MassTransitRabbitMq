# MassTransit RabbitMQ Producer/Consumer Demo

This README explains the Producer/Consumer example using **MassTransit** with **RabbitMQ**.

---

## 📌 Overview

MassTransit simplifies messaging with RabbitMQ by handling:
- Connections
- Exchanges
- Queues
- Bindings
- Serialization
- Consumer conventions

You only focus on:
✔ Publishing messages  
✔ Consuming messages  
✔ Defining message contracts  

---

## 🧱 Project Structure

```
MassTransitRabbitMq/
 ├── Producer/
 │    ├── Producer.csproj
 │    └── Program.cs
 ├── Consumer/
 │    ├── Consumer.csproj
 │    └── Program.cs
 ├── docker-compose.yml
 └── README.md   ← this file
```

---

## 🐇 RabbitMQ Setup (Docker)

```
services:
  rabbitmq:
    image: rabbitmq:3.13-management
    container_name: rabbitmq-demo
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
```

Start RabbitMQ:

```
docker compose up -d
```

Open UI:
👉 http://localhost:15672 (guest/guest)

---

## 📤 Producer (MassTransit)

- Creates a bus connection
- Publishes `OrderSubmitted` messages
- Random or user‑entered Order ID

---

## 📥 Consumer (MassTransit)

- Listens to queue `order-submitted-queue`
- Automatically binds to the exchange created by MassTransit
- Prints consumed messages

---

## 📨 Message Contract

MassTransit requires messages to have a namespace:

```csharp
namespace Contracts;

public record OrderSubmitted(Guid OrderId, string CustomerName, decimal Total);
```

---

## 🔍 Why your earlier messages did NOT appear?

Because:
- MassTransit **auto‑creates its own exchange names**
- If message namespace was missing → ❌ publishing fails
- Queues are named differently than raw RabbitMQ examples

MassTransit conventions:
- Exchange: `contracts:order-submitted`
- Queue: `order-submitted-queue`

You only see messages in Consumer queue **after the consumer is running**.

---

## 🪄 MassTransit vs Raw RabbitMQ (Quick Comparison)

| Feature | Raw RabbitMQ Client | MassTransit |
|--------|----------------------|-------------|
| Queue creation | Manual code | Automatic |
| Exchange creation | Manual | Automatic |
| Bindings | Manual | Automatic |
| Serialization | Manual JSON | Built‑in JSON |
| Error queues | Manual setup | Automatic (`_error`, `_skipped`) |
| Retry policies | Manual | Built‑in |
| Code size | Large | Small & clean |
| Recommended for microservices | ❌ Hard | ✔ Easy |

---

## ▶ How to Run the Demo

### 1️⃣ Start RabbitMQ
```
docker compose up -d
```

### 2️⃣ Start Consumer first
```
dotnet run --project Consumer
```

### 3️⃣ Start Producer
```
dotnet run --project Producer
```

### 4️⃣ Enter order IDs  
Or press Enter for random ones.

Consumer will show:

```
[Consumer] Received OrderSubmitted: 8f90c8... Customer: Alex Total: 149.99
```

---

## ✔ What You Learn From This Example

- How MassTransit automatically creates:
  - Exchanges
  - Queues
  - Message bindings
- How message contracts work
- How a clean publish/consume pipeline looks

---
