# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.11"
    }
  }
  required_version = ">= 0.14.9"
}
provider "azurerm" {
  features {}
}
# Generate a random integer to create a globally unique name
resource "random_integer" "ri" {
  min = 10000
  max = 99999
}
# Create the resource group
resource "azurerm_resource_group" "rg" {
  name     = "TestResourceGroup-${random_integer.ri.result}"
  location = "East Asia"
}
# Create the Linux App Service Plan
resource "azurerm_service_plan" "serviceplan" {
  name                = "webapp-asp-${random_integer.ri.result}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "F1"
}
# Create the web app, pass in the App Service Plan ID, and deploy code from a public GitHub repo
resource "azurerm_linux_web_app" "webapp" {
  name                = "webapp-${random_integer.ri.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.serviceplan.id

  site_config {}
}

resource "azurerm_app_service_source_control" "webapp" {
  app_id   = azurerm_linux_web_app.webapp.id
  repo_url = "https://github.com/mee-ironsoftware/BarcodeReaderOnline"
  branch   = "master"
}