terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm",
      version = "~>3.0.2"
    }
  }

  required_version = ">=1.1.0"
}

provider "azurerm" {
  features {

  }
}

resource "azurerm_resource_group" "sbretrysamplerg" {
  name     = "sbretrysamplerg"
  location = "centralindia"
}

resource "azurerm_servicebus_namespace" "msasamplebus" {
  name                = "msasamplebus"
  resource_group_name = azurerm_resource_group.sbretrysamplerg.name
  location            = azurerm_resource_group.sbretrysamplerg.location
  sku                 = "Standard"
}

resource "azurerm_servicebus_queue" "ordercreatedqueue" {
  name         = "ordercreatedqueue"
  namespace_id = azurerm_servicebus_namespace.msasamplebus.id
  max_delivery_count = 3
}

resource "azurerm_servicebus_queue" "ordercreatedretryqueue" {
  name         = "ordercreatedretryqueue"
  namespace_id = azurerm_servicebus_namespace.msasamplebus.id
}

resource "azurerm_servicebus_topic" "ordercreatedtopic" {
  name         = "ordercreatedtopic"
  namespace_id = azurerm_servicebus_namespace.msasamplebus.id
}

resource "azurerm_servicebus_subscription" "fulfilmentsubscription" {
  name = "fulfilment"
  topic_id = azurerm_servicebus_topic.ordercreatedtopic.id
  max_delivery_count = 3
}

resource "azurerm_servicebus_subscription" "deliverysubscription" {
  name = "delivery"
  topic_id = azurerm_servicebus_topic.ordercreatedtopic.id
  max_delivery_count = 3
}

resource "azurerm_servicebus_namespace_authorization_rule" "samplebussendonlyrule" {
  name         = "sendonly"
  namespace_id = azurerm_servicebus_namespace.msasamplebus.id

  listen = false
  send   = true
  manage = false
}

resource "azurerm_servicebus_namespace_authorization_rule" "samplebuslistenonlyrule" {
  name         = "listenonly"
  namespace_id = azurerm_servicebus_namespace.msasamplebus.id

  listen = true
  send   = false
  manage = false
}