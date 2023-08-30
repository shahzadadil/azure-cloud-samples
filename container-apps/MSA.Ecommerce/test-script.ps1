
$headers = @{
    "Ocp-Apim-Subscription-Key" = "9bd9d3b4829145edb1fd7e435982c079"
}

while (1 -eq 1)
{
    Invoke-WebRequest -URI https://msaapimanagement.azure-api.net/orderapi -Method Get -Headers $headers
}