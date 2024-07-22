# Notification lambda

## Overview

The Notification Service is designed to manage and send notifications through various channels, such as SMS, email, and push notifications. This service abstracts multiple messaging providers, such as Twilio, Amazon SNS, and Vonage, to provide a unified interface for sending notifications.

## Background

The purpose of this service is to handle notifications in a resilient and configurable manner, ensuring messages are sent even in the face of provider failures. The service includes mechanisms for provider failover and message retry, with configuration options for enabling/disabling providers and channels as well as managing their priorities.

## Features

- **Multi-Channel Support**: Send notifications via SMS, email, and push notifications.
- **Provider Abstraction**: Interface with multiple messaging service providers (e.g., Twilio, Amazon SNS, Vonage).
- **Resilience and Failover**:
    - Automatic failover to alternate providers if one fails.
    - Retry mechanism if all providers are unavailable.
- **Configurability**:
    - Enable or disable providers and channels.
    - Set priorities for providers and channels.

![Design Idea](idea.png)