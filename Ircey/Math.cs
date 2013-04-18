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
			return new iNumber(Complex.Add(A,B));
		}
		public static iNumber operator -(iNumber A, iNumber B){
			return new iNumber(Complex.Subtract(A,B));
		}
		public static iNumber operator *(iNumber A, iNumber B){
			return new iNumber(Complex.Multiply(A,B));
		}
		public static iNumber operator /(iNumber A, iNumber B){
			return new iNumber(Complex.Divide(A,B));
		}
		public static iNumber operator +(iNumber A, double B){
			return new iNumber(Complex.Add(A,B));
		}
		public static iNumber operator -(iNumber A, double B){
			return new iNumber(Complex.Subtract(A,B));
		}
		public static iNumber operator *(iNumber A, double B){
			return new iNumber(Complex.Multiply(A,B));
		}
		public static iNumber operator /(iNumber A, double B){
			return new iNumber(Complex.Divide(A,B));
		}
		public static implicit operator iNumber(double d){
			return new iNumber(d);
		}
		public static implicit operator iNumber(Complex p){
			return new iNumber(p);
		}
		public static implicit operator Complex(iNumber i){
			return i.C;
		}
		public override string ToString (){
			string RString = String.Format("{0:#,0.############}", Real);
			string IString = String.Format("{0:#,0.############}", Imaginary).Trim(new char[]{'-'});
			if(RString != "0" && IString != "0") {
				return String.Format("{0} {2} {1}i", RString, IString, Imaginary>=0?"+":"-");
			} else if (RString == "0" && IString != "0") {
				return String.Format("{0}i", IString);
			} else {
				return String.Format("{0}", RString);
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
		public static iOperator Addition = new iOperator("+", new Func<iNumber,iNumber,iNumber>((a,b)=>Complex.Add(a,b)));
		public static iOperator Subtraction = new iOperator("-", new Func<iNumber,iNumber,iNumber>((a,b)=>Complex.Subtract(a,b)));
		public static iOperator Multiplication = new iOperator("*", new Func<iNumber,iNumber,iNumber>((a,b)=>Complex.Multiply(a,b)));
		public static iOperator Division = new iOperator("/", new Func<iNumber,iNumber,iNumber>((a,b)=>Complex.Divide(a,b)));
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
		public static iFunction Negative = new iFunction("-", new Func<iNumber,iNumber>((a)=>Complex.Negate(a)));
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
			list.Insert(0,iContainer.OpenParentheses);
			list.Add(iContainer.CloseParentheses);
			short oi = -1;
			short ci = -1;
			while (true) {
				for (short c=0;c<list.Count;c++) {
					if(list[c].Callsign() == 'c' && ((iContainer)list[c]).open == false) {
						ci = c; break;
					}
				}
				for (short o=ci;o<list.Count;o--) {
					if(list[o].Callsign() == 'c' && ((iContainer)list[o]).open == true) {
						oi = o; break;
					}
				}
					bool solved = false;
					short wint=oi;wint++;
					while (!solved) {
					if (list[wint].Callsign()=='n')  {
						wint++;
					} else if (list[wint].Callsign()=='f') {
						list[wint+1] = ((iFunction)list[wint]).Operate((iNumber)list[wint+1]);
						list.RemoveAt(wint);
					} else if (list[wint].Callsign()=='o') {
						if (list[wint+1].Callsign() == 'f') {
							list[wint+2] = ((iFunction)list[wint+1]).Operate((iNumber)list[wint+2]);
							list.RemoveAt(wint+1);
						}
						if (list[wint+1].Callsign() == 'n') {
							list[wint-1] = ((iOperator)list[wint]).Operate((iNumber)list[wint-1],(iNumber)list[wint+1]);
							list.RemoveAt(wint);
							list.RemoveAt(wint);
						}
					} else if (list[wint].Callsign()=='c') {
						if(list[wint-2].Callsign()=='c') {
							solved=true;list.RemoveAt(wint);
							list.RemoveAt(wint-2);
						} else if(list[wint-1].Callsign()=='c'){
							solved=true;
							list.RemoveAt(wint);
							list.RemoveAt(wint-1);
						}
					} else {throw new Exception();}
				}
				if (list.Count == 1) {
					return (iNumber)list[0];
				} else {
					continue;
				}
			}
		}
	}
}

