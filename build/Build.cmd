@echo off
cd %~dp0..

rake -f Build\build-win.rb "build:no_test_build[., MessagingBase.sln, build, full, local, Release]" 
