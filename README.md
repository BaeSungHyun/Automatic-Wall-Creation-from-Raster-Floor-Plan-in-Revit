
## Acknowledgements

 - This project uses model from https://github.com/CubiCasa/CubiCasa5k repository which is under Creative Commons Attribution-NonCommercial 4.0 International License.
 - Python code of this project is written in Ubuntu, Conda environment and cuda (Geforce RTX 3060). Thus, requirements.txt will only work on Ubuntu. 
 - C# code of this project is written in Windows and Visual Studio.
 - The C# code of this project targets Revit 2025, 
 https://help.autodesk.com/view/RVT/2025/ENU/?guid=Revit_API_Revit_API_Developers_Guide_html use this link to set up addin file.

 - 이 프로젝트는 Creative Commons Attribution-NonCommercial 4.0 International License 하에 있는 https://github.com/CubiCasa/CubiCasa5k 리포지토리의 모델을 사용합니다.
 - 이 프로젝트의 Python 코드는 Ubuntu, Conda 환경 및 CUDA (Geforce RTX 3060)에서 작성되었습니다. 따라서 requirements.txt 파일은 Ubuntu에서만 작동합니다.
 - 이 프로젝트의 C# 코드는 Windows와 Visual Studio에서 작성되었습니다.
 - 이 프로젝트의 C# 코드는 Revit 2025을 대상으로 합니다. https://help.autodesk.com/view/RVT/2025/ENU/?guid=Revit_API_Revit_API_Developers_Guide_html 링크를 사용하여 애드인 파일을 설정하세요.

# Automatic Wall Creation from Raster Floor Plan in Revit

This project consists of two main stages.\
First, use model in Python that inputs raster image of floor plan and outputs csv file containing wall coordinates.\
Second, use csv file in the first stage to automatically create wall using Revit Addin in C#. 
\
\
이 프로젝트는 총 두 단계로 이루어져 있습니다.\
첫 번째 단계에서는 Python에서 모델을 사용하여 평면도의 래스터 이미지를 입력받아 벽 좌표를 포함하는 csv 파일을 출력합니다.\
두 번째 단계에서는 첫 번째 단계의 csv 파일을 사용하여 Revit Addin에서 C#으로 벽을 자동으로 생성합니다.



## Usage/Examples

Fork or clone the repository.\
리포지토리를 포크하거나 클론하세요.

1. To use ai model in python, set up the environment using requirements.txt and conda.\
1. Python에서 AI 모델을 사용하려면 requirements.txt와 conda를 사용하여 환경을 설정하세요.
\


```python
conda create --name myenv --file requirements.txt
python main.py --image "input floor plan image path" --output "output csv file path"
```
\
After running the code, make sure the csv file is located at your desired path.\
코드를 실행한 후, csv 파일이 원하는 경로에 있는지 확인하세요.

\
2. Place 'Revit_Addin_Code/AutomaticWallCreationFromCSV.addin' into your Revit addin path, which is specified in Revit documentation. (https://help.autodesk.com/view/RVT/2025/ENU/?guid=Revit_API_Revit_API_Developers_Guide_html) 
\
2.'Revit_Addin_Code/AutomaticWallCreationFromCSV.addin' 파일을 Revi 문서에 명시된 Revit 애드인 경로에 배치하세요.
\
\
In WallCreation.cs, change 'User Variable' to your desired values. Just make sure that file path of your csv file matches.\
WallCreation.cs 파일에서 'User Variable'을 원하는 값으로 변경하세요. csv 파일의 경로가 일치하는지 확인하세요.
\
## Demo


1. Demo video for Python AI model


https://github.com/BaeSungHyun/Automatic-Wall-Creation-from-Raster-Floor-Plan-in-Revit/assets/117246534/9b6d7519-6c03-4dd9-8cf5-c4b565f0393d


  
2. Demo video for Revit Addin 


https://github.com/BaeSungHyun/Automatic-Wall-Creation-from-Raster-Floor-Plan-in-Revit/assets/117246534/c8383bef-0cb4-4a30-a46c-05329aac87dc


\
\
Final Photo of Wall

![Final_Wall](https://github.com/BaeSungHyun/Automatic-Wall-Creation-from-Raster-Floor-Plan-in-Revit/assets/117246534/4adce065-daeb-425b-a217-66bb99cfc733)
