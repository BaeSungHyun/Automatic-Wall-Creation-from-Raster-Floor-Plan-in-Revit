
## Acknowledgements

 - This project uses model from https://github.com/CubiCasa/CubiCasa5k repository which is under Creative Commons Attribution-NonCommercial 4.0 International License.
 - Python code of this project is written in Ubuntu and Conda environment
 - C# code of this project is written in Windows and Visual Studio
 - The C# code of this project targets Revit 2025, 
 https://help.autodesk.com/view/RVT/2025/ENU/?guid=Revit_API_Revit_API_Developers_Guide_html use this link to set up addin file.

# Automatic Wall Creation from Raster Floor Plan in Revit

This project consists of two main stages.\
First, use model in Python that inputs raster image of floor plan and outputs csv file containing wall coordinates.\
Second, use csv file in the first stage to automatically create wall using Revit Addin in C#. 







## Usage/Examples

Fork or clone the repository.

1. To use ai model in python, set up the environment using requirements.txt and conda.

```python
python main.py --image "input floor plan image path" --output "output csv file path"
```
\
After running the code, make sure the csv file is located at your desired path.

\
2. Place 'Revit_Addin_Code/AutomaticWallCreationFromCSV.addin' into your Revit addin path, which is specified in Revit documentation. (https://help.autodesk.com/view/RVT/2025/ENU/?guid=Revit_API_Revit_API_Developers_Guide_html) 
\
\
In WallCreation.cs, change 'User Variable' to your desired values. Just make sure that file path of your csv file matches.
## Demo

Insert gif or link to demo

