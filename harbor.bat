@echo off

set host=registry.prm.dev.it-serv.ru
set harbor=%host%/mlk

REM login & push
docker login %host% 
docker push %harbor%/mlk_idt_srv:0.1
docker push %harbor%/mlk_idt_admin:0.1

