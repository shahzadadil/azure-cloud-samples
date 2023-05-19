## Message Deferral
It possible to defer a message to a later point for processing. The message becomes unavailable for receiving. The application has the ownership of receiving the message afterwards using the sequence number or message reference. The message will not be auto delivered.

## Scheduling Message
It ispossible to schedule a message in servie bus. While sending the message, a scheduled time needs to be set. This will prevent the message from being visible on the service bus before the scheduled time. It will appear on the scheduled time. Schedule is specified as an offset time.

## Retry
Retries are performed when there is an error processing the mesage and message does not complete. Service Bus retries to deliver the message depending on the number if retries configured for it. Retried can be set to try after a fixed interval or an inceasing interval with each retry attempt.
It only works with ransient errors that happen internally in the SDK, not application thrown errors.  