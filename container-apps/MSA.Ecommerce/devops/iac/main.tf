terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm",
      version = "=3.57.0"
    }
  }

  required_version = ">=1.1.0"
}

provider "azurerm" {
  features {

  }
}

resource "azurerm_resource_group" "msacontainerapprg" {
  name     = "msa-container-app"
  location = "centralindia"
}

resource "azurerm_log_analytics_workspace" "msacontainerapplogworkspace" {
  name                = "msa-container-app-log-wspc"
  location            = azurerm_resource_group.msacontainerapprg.location
  resource_group_name = azurerm_resource_group.msacontainerapprg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_container_app_environment" "msacontainerappenv" {
  name                       = "msacomtainerappenv"
  location                   = "southeastasia"
  resource_group_name        = azurerm_resource_group.msacontainerapprg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.msacontainerapplogworkspace.id
}

resource "azurerm_container_app" "msaordercontainerapp" {
  name                         = "msaordercontainerapp"
  container_app_environment_id = azurerm_container_app_environment.msacontainerappenv.id
  resource_group_name          = azurerm_resource_group.msacontainerapprg.name
  revision_mode                = "Single"
  
  identity{
    type = "SystemAssigned"
  }

  template {
    container {
      name   = "msaorderapi"
      image  = "msaorderapi:dev"
      cpu    = 0.25
      memory = "0.5Gi"
    }
  }
}

