using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polynomial {
	public Polynomial Copy()
	{
		Polynomial newPolynomial = new Polynomial();

		foreach (uint power in _polynomials.Keys)
		{
			newPolynomial._polynomials[power] = _polynomials[power];
		}
		newPolynomial._degree = _degree;

		return newPolynomial;
	}
	Dictionary<uint, double> _polynomials = new Dictionary<uint, double>();
	private uint? _degree = null;
	public uint Degree
	{
		get
		{
			if (!_degree.HasValue)
			{
				_degree = 0;
				foreach (uint factor in _polynomials.Keys)
				{
					_degree = Math.Max(_degree.Value, factor);
				}
			}
			return _degree.Value;
		}
	}
	public double GetFactor(uint power)
	{
		double factor;
		if (_polynomials.TryGetValue(power, out factor))
			return factor;
		throw new ArgumentException("power");
	}
	public void Add(uint power, double factor)
	{
		if (_polynomials.ContainsKey(power))
		{
			_polynomials[power] += factor;
			if (Math.Abs(_polynomials[power]) < Constants.PolynomialEpsilon)
			{
				_polynomials.Remove(power);
			}
		}
		else if (factor != 0)
		{
			_polynomials[power] = factor;
		}

		_degree = null;
	}
	public void Add(Polynomial polynomial)
	{
		foreach (uint power in polynomial._polynomials.Keys)
		{
			Add(power, polynomial._polynomials[power]);
		}
		_degree = null;
	}

	public void Sub(uint power, double factor)
	{
		Add(power, -factor);
		_degree = null;
	}
	public void Sub(Polynomial polynomial)
	{
		foreach (uint power in polynomial._polynomials.Keys)
		{
			Sub(power, polynomial._polynomials[power]);
		}
		_degree = null;
	}
	public Polynomial Mult(uint power, double factor)
	{
		Polynomial newPolynomial = new Polynomial();
		foreach (KeyValuePair<uint, double> kvp in _polynomials)
		{
			newPolynomial.Add(kvp.Key + power, kvp.Value * factor);
		}
		return newPolynomial;
	}
	public Polynomial Mult(Polynomial polynomial)
	{
		Polynomial sum = new Polynomial();
		foreach (KeyValuePair<uint, double> kvp in _polynomials)
		{
			sum.Add(polynomial.Mult(kvp.Key, kvp.Value));
		}
		return sum;
	}
	public Polynomial Power(uint power)
	{
		Polynomial result = this.Copy();
		for (int i = 0; i < power - 1; i++)
		{
			result = result.Mult(this);
		}
		return result;
	}
	public void Div(Polynomial polynomial, out Polynomial result, out Polynomial remainder)
	{
		Div(this, polynomial, out result, out remainder);
	}
	public static void Div(Polynomial a, Polynomial b, out Polynomial result, out Polynomial remainder)
	{
		Polynomial numerator = a.Copy();    // thing being divided
		Polynomial denominator = b.Copy();  // thing dividing
		result = new Polynomial();
		// numerator / denominator = result + remainder / denominator

		uint currentDegree;
		double currentFactor;
		do
		{
			currentDegree = numerator.Degree - denominator.Degree;
			currentFactor = numerator.GetFactor(numerator.Degree) / denominator.GetFactor(denominator.Degree);

			numerator.Sub(denominator.Mult(currentDegree, currentFactor));
			result.Add(currentDegree, currentFactor);
		} while (numerator.Degree >= denominator.Degree);

		remainder = numerator.Copy();
	}
	public Polynomial Derivative()
	{
		Polynomial newPolynomial = new Polynomial();
		foreach (KeyValuePair<uint, double> kvp in _polynomials)
		{
			if (kvp.Key != 0)
			{
				newPolynomial.Add(kvp.Key - 1, kvp.Value * (kvp.Key));
			}
		}

		return newPolynomial;
	}
	public double Eval(double t)
	{
		double sum = 0;
		foreach (KeyValuePair<uint, double> kvp in _polynomials)
		{
			sum += Math.Pow(t, kvp.Key) * kvp.Value;
		}
		return sum;
	}
	public Polynomial Eval(Polynomial polynomial)
	{
		Polynomial sum = new Polynomial();
		foreach (KeyValuePair<uint, double> kvp in _polynomials)
		{
			if (kvp.Key != 0)
			{
				sum.Add(polynomial.Power(kvp.Key).Mult(0, kvp.Value));
			}
			else
			{
				Polynomial constPolynomial = new Polynomial();
				constPolynomial.Add(kvp.Key, kvp.Value);
				sum.Add(constPolynomial);
			}
		}
		return sum;
	}
	public override string ToString ()
	{
		string str = "";
		foreach (var factor in _polynomials)
		{
			if (str == "")
				str += factor.Value + "*x^" + factor.Key;
			else
				str += "+" + factor.Value + "*x^" + factor.Key;
		}
		return str;
	}
}
