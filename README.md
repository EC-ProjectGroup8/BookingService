# Booking Service API

This is a simple API for handling bookings for workout sessions. This project is the first MVP (Minimum Viable Product). No JWT validation/Authentication/Authorization.

## API Endpoints

### Create a Booking

Creates a new booking for a user for a specific workout.

* **URL:** `/api/bookings`
* **Method:** `POST`
* **Auth required:** No

#### Request Body

The request body must be a JSON object with the following properties:

| Name              | Type   | Description                                                                                                                               |
| :---------------- | :----- | :---------------------------------------------------------------------------------------------------------------------------------------- |
| `UserEmail`       | string | **Required**. The email of the user making the booking. The client application (e.g., the React app) is responsible for providing this. |
| `WorkoutIdentifier` | string | **Required**. The unique string identifier for the workout session the user wants to book.                                                |

#### Example Request

```json
{
  "UserEmail": "test.user@example.com",
  "WorkoutIdentifier": "strength-class-101"
}

## Successful Response (201 Created) + (200 That it connected to the API)
### If the booking is successful, the API returns the created booking object.

