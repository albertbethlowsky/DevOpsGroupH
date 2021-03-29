#Run this script, to get excel file with recommendations to make our azure subcription with its contents more secure. 

#Installation Guide: https://azsk.azurewebsites.net/00a-Setup/Readme.html
Install-Module AzSK -Scope CurrentUser -AllowClobber -Force -SkipPublisherCheck

#AzSK: Subcription Health Check: https://azsk.azurewebsites.net/01-Subscription-Security/Readme.html#overview
Get-AzSKSubscriptionSecurityStatus -SubscriptionId 42954916-e733-465e-97fb-9dcbcf102b04

#Scan the resources inside the subscription
#https://azsk.azurewebsites.net/02-Secure-Development/Readme.html
Get-AzSKAzureServicesSecurityStatus -SubscriptionId 42954916-e733-465e-97fb-9dcbcf102b04 -ResourceGroupNames NeutralsRG

#THE LOG IS SAVED LOCALLY ON THE COMPUTER (SEE IN TERMINAL)