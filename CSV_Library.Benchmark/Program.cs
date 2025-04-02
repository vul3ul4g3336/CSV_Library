// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using CSV_Library.Benchmark;

var summary = BenchmarkRunner.Run<Span_VS_NotUseArray>();

