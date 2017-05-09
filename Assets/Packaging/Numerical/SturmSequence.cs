using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SturmSequence {
	List<Polynomial> _polynomials = new List<Polynomial>();

	public SturmSequence(Polynomial polynomial)
	{
		if (polynomial.Degree == 0)
		{
			throw new ArgumentException("polynomial");
		}
		_polynomials.Add(polynomial.Copy());
		_polynomials.Add(polynomial.Derivative());
		Polynomial atK = _polynomials[1];
		Polynomial atKMinus1 = _polynomials[0];
		Polynomial newPolynomial;
		Polynomial result;
		while (atK.Degree > 0)
		{
			Polynomial.Div(atKMinus1, atK, out result, out newPolynomial);
			newPolynomial = newPolynomial.Mult(0, -1);
			_polynomials.Add(newPolynomial);
			atKMinus1 = atK;
			atK = newPolynomial;
		}
	}
	private int FindNumberOfSignChanges(double value)
	{
		int numberOfSignChanges = 0;
		double currentValue;
		double oldValue = Math.Round(_polynomials[0].Eval(value), Constants.DecimalPlaces);
		double oldSign = Math.Sign(oldValue);
		double currentSign;
		for (int i = 1; i < _polynomials.Count; i++)
		{
			currentValue = Math.Round(_polynomials[i].Eval(value), Constants.DecimalPlaces);
			currentSign = Math.Sign(currentValue);
			if (currentSign * oldSign < 0)
			{
				numberOfSignChanges++;
			}
			if (currentSign != 0)
			{
				oldSign = currentSign;
			}
		}
		return numberOfSignChanges;
	}
	private int NumberOfRootsInRange(double min, double max)
	{
		return Math.Abs(FindNumberOfSignChanges(min) - FindNumberOfSignChanges(max));
	}
	public List<double> FindRoots(double min, double max)
	{
		int numberOfRoots = NumberOfRootsInRange(min, max);
		if (numberOfRoots == 0)
		{
			return null;
		}
		double difference = max - min;
		double midpoint = (min + max) / 2.0;

		if (Math.Abs(difference) < Constants.PolynomialEpsilon)
		{
			if (numberOfRoots == 1)
			{
				List<double> singleRoot = new List<double>();
				singleRoot.Add(Math.Round(midpoint, Constants.DecimalPlaces));
				return singleRoot;
			}
			else
			{
				return null;
			}
		}

		List<double> foundRoots;
		List<double> roots = new List<double>();

		foundRoots = FindRoots(min, midpoint);
		if (foundRoots != null)
		{
			roots.AddRange(foundRoots);
			if (roots.Count == numberOfRoots)
			{
				return roots;
			}
		}
		foundRoots = FindRoots(midpoint, max);
		if (foundRoots != null)
		{
			roots.AddRange(foundRoots);
		}
		if (roots.Count != 0)
		{
			return roots;
		}
		return null;
	}
}
