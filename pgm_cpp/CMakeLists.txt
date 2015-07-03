 cmake_minimum_required(VERSION 2.8 FATAL_ERROR)

 project (pgm_cpp)

 set(CMAKE_BUILD_TYPE Release)

 add_library(pgm_cpp SHARED rbm.cpp rbm.h)