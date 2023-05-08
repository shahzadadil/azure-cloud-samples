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
  sku                 = "Basic"
}

resource "azurerm_servicebus_queue" "platformeventqueue" {
  name = "platformeventqueue"
  namespace_id = azurerm_servicebus_namespace.msasamplebus.id
}