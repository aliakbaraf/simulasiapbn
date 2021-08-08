publish:
	dotnet publish \
		--configuration Release \
		--framework net5.0 \
		--no-self-contained \
		-p:PublishReadyToRun=true \
		--runtime=win-x64

linux:
	dotnet publish \
		--configuration Release \
		--framework net5.0 \
		--no-self-contained \
		-p:PublishReadyToRun=true \
		--runtime=linux-x64

macos:
	dotnet publish \
		--configuration Release \
		--framework net5.0 \
		--no-self-contained \
		-p:PublishReadyToRun=true \
		--runtime=osx-x64

clean:
	dotnet clean \
		--configuration Release
