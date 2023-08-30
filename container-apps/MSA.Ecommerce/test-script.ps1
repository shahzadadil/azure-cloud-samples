
$headers = @{
    "Ocp-Apim-Subscription-Key" = ""
}

while (1 -eq 1)
{
    Invoke-WebRequest -URI https://msaapimanagement.azure-api.net/orderapi -Method Get -Headers $headers
}