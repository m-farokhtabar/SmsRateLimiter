# SMS Rate Limiter Microservice

## Overview

This microservice is designed to handle rate-limiting for sending SMS messages from businesses to customers, ensuring that the external provider's API limits are respected before any SMS is sent. The service uses a **Sliding Window Log** algorithm to ensure that message sending limits are not exceeded for both a specific business phone number and across the entire account. The microservice acts as a gatekeeper, preventing unnecessary API calls and associated costs.

## Features

- **Rate Limiting**: 
  - **Per Phone Number**: Maximum number of messages per second that can be sent from a single business phone number.
  - **Account-wide Limit**: Maximum number of messages that can be sent across the entire account per second.
  
- **Sliding Window Log Algorithm**: This algorithm ensures the rate limit is applied while respecting the time window of requests.

- **Cleanup Service**: A service that periodically cleans up records of phone numbers that are no longer active or haven't been used for a period of time.

- **Local Memory**: All data is stored in memory for simplicity, with no external databases or Redis required.

- **Swagger Integration**: The project includes Swagger for easier exploration and testing of the WebAPI.

## Project Structure

- **SmsRateLimiter.API**: The WebAPI project containing the controllers and configuration for the rate limiter.
- **SmsRateLimiter.Service**: The service project where the logic for rate limiting and cleanup is implemented.
  
  All dependency injection and configuration for services are managed within their respective projects.

## Configuration

The configuration is managed in the `appsettings.json` file located in the **SmsRateLimiter.API** project. The important settings are:

### Rate Limiting Settings

```json
"RateLimiterSettings": {
  "PhoneNumberLimit": 5,
  "AccountLimit": 10,
  "WindowSize": "00:00:01"
}
```
- **PhoneNumberLimit**: Maximum number of messages that can be sent from a single business phone number per second.
- **AccountLimit**: Maximum number of messages that can be sent across the entire account per second.
- **WindowSize**: Time window within which the limits are applied (e.g., 1 second).
```json
"CleanupServiceSetting": {
  "Period": 30
}
```
- **Period**: Frequency in seconds at which the cleanup service runs to remove inactive phone number logs.
## API Endpoints

1. **Send**: Check if a message can be sent without exceeding the rate limit for the business phone number.  
   **URL**: `/api/Message/Send`  
   **Method**: `POST`  
   **Request Body**:  
   - JSON payload to check the rate limit for a given business phone number.  
   **Response**:  
   - `200 Status` if the message can be sent.  
   - `429 Status` if the rate limit has been exceeded.

2. **GetAccountLogsPerTime**: Get the rate of successful requests for the account in a specified period.  
   **URL**: `/api/Message/status/accountlog`  
   **Method**: `GET`  
   **Response**:  
   - Rate of successful requests for the account.

3. **GetPhoneLogsPerTime**: Get the rate of successful requests for a specific phone number in a given period.  
   **URL**: `/api/Message/status/phonelog/{phoneNumber}`  
   **Method**: `GET`  
   **Query Parameters**:  
   - `phoneNumber`: The business phone number to check.  
   **Response**:  
   - Rate of successful requests for the selected phone number.

4. **GetPhoneLogsByDate**: Get a list of successful requests for a specific phone number within a date range.  
   **URL**: `/api/Message/status/phonelog/{phoneNumber}/{from}/{to}`  
   **Method**: `GET`  
   **Query Parameters**:  
   - `phoneNumber`: The business phone number.  
   - `startDate`: Start datetime for the period(UTC format).  
   - `endDate`: End datetime for the period(UTC format).  
   **Response**:  
   - List of successful requests from the specified phone number within the date range.

## Performance Expectations

- The microservice is designed to handle a high volume of requests efficiently.
- It uses local memory for data storage (no external databases or Redis).
- Resource management is handled by a cleanup service, which ensures that phone number logs are not kept indefinitely.
- The microservice must respect rate limits in real-time, preventing unnecessary API calls to the external messaging provider.

## Testing

- **Moq** is used for unit testing the rate limiter logic.
- Tests cover various scenarios, including:
  - Checking whether a message can be sent when limits are near or exceeded.  

## Getting Started

### Prerequisites

- .NET 8 SDK or later.
- Visual Studio or your preferred IDE.

### Running the Application

1. Clone this repository to your local machine.
2. Open the solution in your IDE.
3. Run the `SmsRateLimiter.API` project.
4. Navigate to `https://localhost:7215/swagger` in your browser to interact with the API via Swagger.

### Configuration for Development

In `appsettings.json`, you can adjust the following parameters:

- **PhoneNumberLimit**: Change the limit of messages per phone number.
- **AccountLimit**: Change the limit of messages for the entire account.
- **WindowSize**: Adjust the time window for rate limiting.
- **Cleanup Period**: Adjust the frequency of the cleanup service.

## Notes

- The service currently does not handle exceptions.
- The project uses local memory instead of Redis or any external databases, so data will be lost if the service restarts.
- The rate limiter works using the Sliding Window Log algorithm, ensuring that message requests are processed in a way that respects the configured limits.

## Future Improvements

- Implement exception handling for better robustness.
- using Redis for distributed rate limiting, enabling efficient scaling of the system across multiple instances.
- Integrate persistent storage for saving log requests, improving data control, and enabling better management of related queries.
- Implement more complex cleanup strategies based on phone number activity.
