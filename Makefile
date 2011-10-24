
SOURCES=src/Board.cs src/BoardLoader.cs src/Config.cs src/FroggerAnimation.cs src/Frogger.cs src/FroggerObjects.cs src/Graphics.cs src/Utils.cs


frogger.exe : $(SOURCES)
	gmcs -pkg:dotnet -pkg:tao-sdl $(SOURCES) -out:frogger.exe
