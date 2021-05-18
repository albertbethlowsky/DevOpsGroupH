# SLA for Neutrals Minitwit

This Agreement represents a Service Level Agreement ("SLA" or "Agreement") between group Neutrals and the users of the social media platform Minitwit.™

This Agreement remains valid until superseded by a revised agreement mutually endorsed by the stakeholders.

This Agreement outlines the parameters of all IT services covered as they are mutually understood by the primary stakeholders. This Agreement does not supersede current processes and procedures unless explicitly stated herein.

## Stakeholders

IT service provider: Group Neturals

IT service user(s): Users of the social media platform Minitwit.™

# Metrics
## Uptime
* ~96% since 19th of march. Downtime is measured by 1 minute intervals of CPU time. 

## Response Time
* The average response time since 19th of march is ~7.64 seconds, because of a long period of database errors from 28 of march to the 7th of april. Our database were missing users created from the simulator. On the 7th of april, we received an sql dump from another team which resolved our http 500 errors. 
* The average response time from the 7th of april is ~101.37 ms. 

## Recovery Time
Automatic Technical Recovery Time (Azure rebooting): ~5 min
Manual recovery time (identifying error and solving): ~1 hour

## Failure frequency
* Simulator status interface: http://138.68.93.2/status.html 

## References
* https://www.slatemplate.com/
* Many of the metrics are collected using azure's metrics service application: https://docs.microsoft.com/en-us/azure/azure-monitor/essentials/data-platform-metrics
    * We are also using grafana+prometheus for monitoring, but since it was implemented later in the development phase, it does not include as much insight as azure's integrated services. 
* Azure Devops SLA: https://azure.microsoft.com/en-us/support/legal/sla/azure-devops/v2_0/

