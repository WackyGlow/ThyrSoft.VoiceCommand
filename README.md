# Real-time Speech Recognition with Microsoft Cognitive Services

This repository hosts a C# application that showcases real-time speech recognition using Microsoft Cognitive Services. With this application, you can capture audio from your default microphone input, process it using the Speech SDK, and visualize the recognized text directly in your console.

## Getting Started

To set up and run this application on your local machine, follow these detailed instructions:

### 1. Clone the Repository

Begin by cloning this repository to your local machine. Open your terminal or command prompt and execute the following command:

```bash
git clone https://github.com/yourusername/speech-recognition.git
```

### 2. Set up Microsoft Cognitive Services

Before running the application, you need to obtain credentials for Microsoft Cognitive Services Speech API. Here's how:

- **Sign Up:** If you haven't already, sign up for Microsoft Cognitive Services.
- **Get Subscription Key and Region:** After signing up, navigate to the Speech section and obtain your subscription key and region.
- **Set Environment Variables:** Set the environment variables `SPEECH_KEY` and `SPEECH_REGION` on your machine. The key should be set to your subscription key, and the region should match the region associated with your subscription.

### 3. Install Dependencies

Make sure you have all the necessary dependencies installed to build and run the application:

- **Speech SDK:** This application relies on the Microsoft Cognitive Services Speech SDK. You can install it via NuGet package manager.

### 4. Build and Run the Application

Once you've set up the environment and installed the dependencies, you're ready to build and run the application. Follow these steps:

- **Open Solution:** Navigate to the directory where you cloned the repository and open the solution file in your preferred development environment (Visual Studio, Visual Studio Code, etc.).
- **Build Solution:** Build the solution to ensure all dependencies are resolved and the application compiles without errors.
- **Run Application:** Start the application. It will start listening for speech input from your default microphone.

## Usage

Here's how you can interact with the application:

- **Speech Recognition:** The application continuously listens for speech input. As you speak into your microphone, the recognized text will be displayed in the console.
- **Silent Periods:** If there's a moment of silence, the application will detect it and reset the recognized text accordingly.
- **Stopping the Application:** To stop the application, simply press any key.

## Configuration

You can customize certain aspects of the application:

- **Language:** Adjust the language for speech recognition by modifying the `SpeechRecognitionLanguage` variable in the code.
