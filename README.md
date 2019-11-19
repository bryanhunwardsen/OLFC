# OLFC
Fractional Calculator

The high level algoritm is as follows:

Run Console UI loop, catching all expected errors gracefully, enabling user query input

Take user input and parse to simple operation of "operand1 operator operand2"
Various levels of error checking

Convert and store all operands as improper factions to ease calculation
Various Error checking

Perform Mathematic operations +,-,* while monitoring for integer overflow

Perform Division while monitoring for integer overflow and undefined(defivide by zero instancesa)

Store the result as improper praction

Return result as whole number, if possible, mixed number if possible, then as fraction as last resort

Basic Layers of error checking:
Iput validation meets specified format and only contains unsigned int values as components, only recognized operands
Catch any intermediate operation or final result that might result in an Int overflow 

Write test cases for known error scenarios.

