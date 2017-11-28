@echo off
cd %~dp0\build\site
start http://localhost:8080/api/DevExtreme.AspNet.Data.html
..\docfx.console\tools\docfx serve
