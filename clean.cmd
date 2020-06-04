rmdir .\.vs /s /q

rmdir .\ZRLib\bin /s /q
rmdir .\ZRLib\obj /s /q
rmdir .\ZRLib\ProfilingSessions /s /q

rmdir .\ZRLib.Tests\bin /s /q
rmdir .\ZRLib.Tests\obj /s /q
rmdir .\ZRLib.Tests\ProfilingSessions /s /q

cd .\MysteriesRL
call clean.cmd
cd ..
