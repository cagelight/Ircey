using System;
using System.Collections.Generic;

namespace Ircey
{
	public interface IMathematical { char Callsign(); }

	public struct iNumber : IMathematical {
		double nN;
		double iN;
		public iNumber(double nN = 0, double iN = 0) {
			this.nN = nN;
			this.iN = iN;
		}
		public static iNumber operator +(iNumber A, iNumber B){
			return new iNumber(A.nN + B.nN, A.iN + B.iN);
		}
		public static iNumber operator -(iNumber A, iNumber B){
			return new iNumber(A.nN - B.nN, A.iN - B.iN);
		}
		public static iNumber operator *(iNumber A, iNumber B){
			return new iNumber(A.nN * B.nN, A.iN * B.iN);
		}
		public static iNumber operator /(iNumber A, iNumber B){
			return new iNumber(A.nN / B.nN, A.iN / B.iN);
		}
		public static iNumber operator +(iNumber A, double B){
			return new iNumber(A.nN + B);
		}
		public static iNumber operator -(iNumber A, double B){
			return new iNumber(A.nN - B);
		}
		public static iNumber operator *(iNumber A, double B){
			return new iNumber(A.nN * B);
		}
		public static iNumber operator /(iNumber A, double B){
			return new iNumber(A.nN / B);
		}
		public static implicit operator iNumber(double d){
			return new iNumber(d);
		}
		public static implicit operator double(iNumber i){
			return i.nN;
		}
		public override string ToString (){
			if(iN != 0) {
				return String.Format("{0} + {1}i", nN, iN);
			} else {
				return String.Format("{0}", nN);
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
		public static iOperator Power = new iOperator("^", new Func<iNumber,iNumber,iNumber>((a,b)=>Math.Pow(a,b)));
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
		public static iFunction Sqrt = new iFunction("sqrt", new Func<iNumber,iNumber>((a)=>Math.Sqrt(a)));
		public static iFunction[] StandardFunctions = new iFunction[] {Negative, Sqrt};
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
			return iOperator.Addition.Operate(5D, 6D);
		}
	}
}

