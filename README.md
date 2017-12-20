# DuraShop <img align="left" width="50" height="50" src="https://github.com/jedjohan/durashop/blob/master/WebFront/wwwroot/images/durashop-small.png">

Brown Bag project to learn about Azure Durable Functions. DuraShop is Micro Service oriented and contains some basic functionality for a serverless e-commerce site, currently these components are included:

* **DuraShoppingCart**: Orchestrator to store a customers Shopping Cart. Implements _**External Events**_
* **DuraShopMFA**: Used for 2 factor authentication with codes in SMS. Implements _**Human Interaction**_
* **DuraShopUser**: Orchestrator to store user data. Implements _**External Events**_ and _**Stateful Singleton**_
* **DuraShopCheckOut**: Orchestrator to handle Checkout (payment/order/notification). Implements _**Function Chaining**_
* **Duracommunication**: Listens to Event Grid and sends mail/SMS. Regular Function
* **SPA Front End**: Emulate a simple e-shop

## Getting Started

Clone the repository and examine code. Note that you need to edit configurations in local.settings.json for each Azure Function
### Prerequisites

* Account in Twilio for testing the Multi Factor Authentication (SMS codes)
* Microsoft Azure Storage Emulator (up and running) (useful to clear in between debugging sessions with "Azurestorageemulator clear all")
### Installing

Clone and run...  nah, its a bit more complex really

## Deployment

Deploy to Azure. I prepared empty Function Apps and selected "Publish to Existing Function" when deploying.

## Authors

* [Robin](https://github.com/RobinNord)
* [Johan](https://github.com/jedjohan)

## Acknowledgments

https://github.com/marcduiker/

