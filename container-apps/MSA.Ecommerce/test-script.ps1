
$headers = @{
    "Ocp-Apim-Subscription-Key" = "29fc2fa9db304796a2fed78cea70806f"
}

while (1 -eq 1)
{
    Invoke-WebRequest -URI https://msaapim.azure-api.net/order/api/order -Method Post -Headers $headers
}