# Altium test task

## Overview

Solution consists of three projects:

- ### Generator

  -  Generates a test file with format:  Number. Text

  -  Configurable parameters: path to output file, size of file in GB, maximum line length, duplicate chance percent

  -  Example usage:

         D:\generated-file.txt 1 100 5

- ### Sorter

    -  Sorts input file using external merge sort:
       - Chunking file by allowed buffer size, sorts each chunk and writes to file
       - Merging sorted chunks into the output file

    -  Configurable parameters: path to input file, path to output file, allowed in memory buffer size in GB    

    -  Example usage:
            
           D:\unsorted-file.txt D:\sorted-file.txt 0.5
    - Recommended buffer size = about 1/5 - 1/6 of free RAM

- ### Tests

    - Some tests :) 

## Tradeoffs
Somewhat sacrificed SOLID principles and made some assumptions about file sizes in favor of performance, as performance was first priority, according to the task.