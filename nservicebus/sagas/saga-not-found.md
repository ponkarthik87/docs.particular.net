---
title: Sagas Not Found
summary: How a message is handled when it could be executed by a saga but no saga could be found.
component: Core
reviewed: 2021-08-13
related:
- samples/saga
---

The messages which are handled by sagas can either start a new saga (if handled by `IAmStartedByMessages<T>`) or update an existing saga (if handled by `IHandleMessages<T>`). If the incoming message in meant to be handled by a saga but is not expected to start a new one, then NServiceBus uses [correlation rules](/nservicebus/sagas/#correlating-messages-to-a-saga) to find an existing saga. If no existing saga can be found, all implementations of `IHandleSagaNotFound` are executed. If no implementation can be found, the message is discarded without additional notification.

snippet: saga-not-found

Note that in the example above the message will be considered successfully processed and sent to the audit queue even if no saga was found. Throw an exception from the `IHandleSagaNotFound` implementation to move the message to the error queue.

Note: If there are multiple saga types that handle a given message type and one of them is found while others are not, the `IHandleSagaNotFound` handlers **will not be executed**. The `IHandleSagaNotFound` handlers are executed only if no saga instances are invoked. The following table illustrates when the `IHandleSagaNotFound` handlers are invoked in cases when a message is mapped to two different saga types, A and B.

| Saga A found | Saga B found | Not found handler invoked |
|--------|--------|---------|
| ✔️    | ✔️     | ❌     |
| ✔️    | ❌     | ❌     |
| ❌    | ✔️     | ❌     |
| ❌    | ❌     | ✔️     |

include: non-null-task

The ability to provide an implementation for `IHandleSagaNotFound` is especially useful if compensating actions are needed for messages which arrive after the saga has been marked as complete. This is a common scenario when using timeouts inside the saga.

For example, consider a saga that is used for managing the registration process on the website. After a customer registers, they receive an email with a confirmation link. The system will wait for confirmation for a specific period of time, e.g. 24 hours. If the user doesn't click the link within 24 hours, their data is removed from the system and saga is completed. However, they might decide to click the confirmation link a few days later. In this case, the related saga instance can't be found and an exception will be thrown. By implementing `IHandleSagaNotFound` it is possible to handle the situation differently, e.g. redirect the user to the registration website and ask them to fill out the form again.

The implementation of `IHandleSagaNotFound` should be driven by the business requirements for a specific situation. In some cases the message might be ignored; in others, it might be useful to track whenever that situation happens (e.g. by logging or sending another message). In still other cases, it might make sense to perform a custom compensating action.
