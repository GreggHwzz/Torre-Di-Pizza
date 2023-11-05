# Torre Di Pizza Order Management System

## Overview

Torre Di Pizza is an order management system designed to facilitate the handling of pizza orders from entry through to kitchen preparation and finally to delivery. The system is divided into three main forms, each managing a specific part of the order process. This application demonstrates the integration of Windows Forms for user interactions and RabbitMQ for message queuing capabilities.

## Features

- `Form1`: Order entry form where customer details and pizza choices are inputted.
- `Form2`: Kitchen order management form that displays incoming orders and manages their state.
- `Form3`: Notification form to alert staff when orders are ready and to manage customer communication.

## Requirements

To run this project, you will need:

- .NET Framework 4.7.2 or higher
- RabbitMQ server running locally or accessible over the network
- Newtonsoft.Json package for JSON serialization/deserialization

## Setup

1. Ensure RabbitMQ is installed and running. By default, the application will try to connect to a local RabbitMQ server.
2. Open the solution in Visual Studio.
3. Restore NuGet packages to resolve any missing dependencies.
4. Build the solution to ensure all projects compile correctly.

## Running the Application

1. Open your terminal and execute `dotnet run`.
2. Start by running `Form1` to enter and submit new pizza orders.
3. Open `Form2`, which will automatically start listening for new orders from the RabbitMQ queue and display them.
4. Run `Form3` to handle order notifications and to perform customer-related actions.

### Executing Forms

In Visual Studio:

- Set multiple startup projects by going to `Solution Properties -> Common Properties -> Startup Project`.
- Choose 'Start' action for `Form1`, `Form2`, and `Form3`.
- Click 'OK', and then start the solution.

Alternatively, you can manually start each form from the Visual Studio debugger or by navigating to the output directory (`/bin/Debug/`) and running the executables directly.

## Contributions

To contribute to this project, please create a fork and submit a pull request with your proposed changes.

## License

This project is licensed under the MIT License - see the LICENSE.md file for details.

---

**Note**: This README assumes that the user has a basic understanding of Windows Forms applications and RabbitMQ. Adjust the content accordingly based on the target users' expertise level.