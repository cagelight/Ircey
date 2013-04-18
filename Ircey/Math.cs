using System;
using System.Numerics;
using System.Collections.Generic;

namespace Ircey
{
	public interface IMathematical { char Callsign(); }

	public struct iNumber : IMathematical {
		public double Real {get{return C.Real;}}
		public double Imaginary {get{return C.Imaginary;}}
		public Complex C;
		public iNumber(double Real = 0, double Imaginary = 0) {
			this.C = new Complex(Real, Imaginary);
		}
		public iNumber(Complex p) {
			this.C = p;
		}
		public static iNumber operator +(iNumber A, iNumber B){
			return new iNumber(A.Real + B.Real, A.Imaginary + B.Imaginary);
		}
		public static iNumber operator -(iNumber A, iNumber B){
			return new iNumber(A.Real - B.Real, A.Imaginary - B.Imaginary);
		}
		public static iNumber operator *(iNumber A, iNumber B){
			return new iNumber(A.Real * B.Real, A.Imaginary * B.Imaginary);
		}
		public static iNumber operator /(iNumber A, iNumber B){
			return new iNumber(A.Real / B.Real, A.Imaginary / B.Imaginary);
		}
		public static iNumber operator +(iNumber A, double B){
			return new iNumber(A.Real + B);
		}
		public static iNumber operator -(iNumber A, double B){
			return new iNumber(A.Real - B);
		}
		public static iNumber operator *(iNumber A, double B){
			return new iNumber(A.Real * B);
		}
		public static iNumber operator /(iNumber A, double B){
			return new iNumber(A.Real / B);
		}
		public static implicit operator iNumber(double d){
			return new iNumber(d);
		}
		public static implicit operator double(iNumber i){
			return i.Real;
		}
		public static implicit operator iNumber(Complex p){
			return new iNumber(p);
		}
		public static implicit operator Complex(iNumber i){
			return i.C;
		}
		public override string ToString (){
			if(Imaginary != 0 && Real != 0) {
				return String.Format("{0} {2} {1}i", Real, Imaginary.ToString().Trim(new char[]{'-'}), Imaginary>=0?"+":"-");
			} else if (Real == 0 && Imaginary != 0) {
				return String.Format("{0}i", Imaginary);
			} else {
				return String.Format("{0}", Real);
			}
			
		}
		public char Callsign() {return 'n';}
		public static iNumber Zero = new iNumber(0);
	}

	public struct iOperator : IMathematical {
		public readonly string sign;
		public readonly Func<iNumber, iNumber, iNumber> operation;
		public iOperator (string sign, Func<iNumber, iNumber, iNumber> oper) {
			this.sign = sign;
			this.operation = oper;
		}
		public iNumber Operate (iNumber A, iNumber B) {
			return operation(A, B);
		}
		public override string ToString (){
			return sign;
		}
		public char Callsign() {return 'o';}
		public static iOperator Addition = new iOperator("+", new Func<iNumber,iNumber,iNumber>((a,b)=>a+b));
		public static iOperator Subtraction = new iOperator("-", new Func<iNumber,iNumber,iNumber>((a,b)=>a-b));
		public static iOperator Multiplication = new iOperator("*", new Func<iNumber,iNumber,iNumber>((a,b)=>a*b));
		public static iOperator Division = new iOperator("/", new Func<iNumber,iNumber,iNumber>((a,b)=>a/b));
		public static iOperator Power = new iOperator("^", new Func<iNumber,iNumber,iNumber>((a,b)=>Complex.Pow(a,b)));
		public static iOperator[] StandardOperators = new iOperator[] {Addition, Subtraction, Multiplication, Division, Power};
	}

	public struct iFunction : IMathematical {
		public readonly string sign;
		public readonly Func<iNumber, iNumber> operation;
		public iFunction (string sign, Func<iNumber, iNumber> oper) {
			this.sign = sign;
			this.operation = oper;
		}
		public iNumber Operate (iNumber A) {
			return operation(A);
		}
		public override string ToString (){
			return sign;
		}
		public char Callsign() {return 'f';}
		public static iFunction Negative = new iFunction("-", new Func<iNumber,iNumber>((a)=>-a));
		public static iFunction Sqrt = new iFunction("sqrt", new Func<iNumber,iNumber>((a)=>Complex.Sqrt(a)));
		public static iFunction Sin = new iFunction("sin", new Func<iNumber,iNumber>((a)=>Complex.Sin(a)));
		public static iFunction Cos = new iFunction("cos", new Func<iNumber,iNumber>((a)=>Complex.Cos(a)));
		public static iFunction Tan = new iFunction("tan", new Func<iNumber,iNumber>((a)=>Complex.Tan(a)));
		public static iFunction[] StandardFunctions = new iFunction[] {Negative, Sqrt, Sin, Cos, Tan};
	}

	public struct iContainer : IMathematical {
		public readonly bool open;
		public readonly string sign;
		public iContainer(string sign, bool open) {
			this.sign = sign;
			this.open = open;
		}
		public override string ToString (){
			return sign;
		}
		public char Callsign() {return 'c';}
		public static iContainer OpenParentheses = new iContainer("(", true);
		public static iContainer CloseParentheses = new iContainer(")", false);
		public static iContainer[] StandardContainers = new iContainer[] {OpenParentheses, CloseParentheses};
	}

	public static class Calculate {
		public static iNumber IMathematicalList (List<IMathematical> list) {
			short oi = -1;
			short ci = -1;
			for (short c=0;c<list.Count;c++) {
				if(list[c].Callsign() == 'c' && ((iContainer)list[c]).open == false) {
					ci = c; break;
				}
			}
			for (short o=ci;o<list.Count;o++) {
				if(list[o].Callsign() == 'c' && ((iContainer)list[o]).open == true) {
					oi = o; break;
				}
			}
			return iOperator.Subtraction.Operate(iFunction.Sqrt.Operate(-6D), new iNumber(3D, 60D));
		}
	}
}

