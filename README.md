# Tyche

This project can help you in gathering data from different files and machines at one to analyse it in future.

## Tyche.Manager

API that gives you ability to manage and store found data for different **Scanners**.   
Uses Redis DB as a primary DB (you can rewrite for other DB).

## Tyche.Scanner

Project module that performs scanning on local machine and sending found data to **Manager**.
