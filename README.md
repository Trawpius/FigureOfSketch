# FigureOfSketch
## Design Platform

Language: C#
Framework: .NET Framework 4.6.1
Output Type: WPF Windows Application

Developed on Windows 10 using Visual Studio 2022

## Function

Program references a "base directory". 

All immediate child directories in the base directory become options available in the combo box.\
Child directory selected in the combo box becomes the active directory.\
Time interval allows user to select between 30s and 600s, in 30s intervals.

"Start" creates a worker thread, which will randomly select and disply an image file from the active directory
  - Accepted file extensions: jpg, jpeg, bmp, png
  - Image displays for X seconds, depending on time interval selected.
  
Pause stops the timer and blurs the image, but keeps the session running.
  - Clicking start resumes the session.
  
Skip will move to the next randomly selected image in active directory\
Stop will stop the timer and end the worker thread

Base Directory is referenced from Config.XML\
Use file dialog (indicated by "..." button) to find and select new base directory
