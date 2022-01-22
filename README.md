# timeclock
First project in C#, use during my microsoft certification.
UI is in FRENCH only (not translated for the moment).
Code is in english.

## Features :
- launch automaticly with Window
- allow you to count work session time
- session can be started and stopped
- auto stop on windows session stop and 10 min of inactivty
- can generate a read only excel report over the last 5 month
- allow work session over 2 days

## Stack
- Made with .Net 4.7.2 Framework
- MVVMLight
- Use NPOI for Excel file generation
- Entity ORM
- SQLite for persistence

## Architecture
- Clean architecture for the business features side
- MVVM for the UI behaviors

Build tested on Visual Studio 2019 and 2022.
