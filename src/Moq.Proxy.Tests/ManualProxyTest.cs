namespace Moq.Proxy.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	public interface ICalculator
	{
		//int this[int x, int y] { get; set; }

		//int this[string s] { get; }

		//int this[int i] { set; }

		int Result { get; set; }

		int Add (int x, int y);

		int Add (int x, int y, int z);

		bool TryAdd (int seed, ref int x, ref int y, out int z);

		void TurnOn ();
	}

	public class Calculator : ICalculator
	{
		public virtual int this[int x, int y]
		{
			get { return 0; }
			set { }
		}

		public virtual int Result { get; set; }

		public virtual int this[string s] => 0;

		public int this[int i] { set { } }


		public virtual int Add (int x, int y) => x + y;

		public virtual int Add (int x, int y, int z) => x + y + z;

		public virtual bool TryAdd (int seed, ref int x, ref int y, out int z)
		{
			z = x + y;
			return true;
		}

		public virtual void TurnOn () { }
	}

	public class CalculatorClassProxy : Calculator, IDisposable, IProxy
	{
		BehaviorPipeline pipeline;
		Calculator target;
		IDisposable disposable;

		public CalculatorClassProxy ()
		{
			pipeline = new BehaviorPipeline ();
		}

		public CalculatorClassProxy (Calculator target)
			: this ()
		{
			this.target = target;
			disposable = target as IDisposable;
		}

		public IList<IProxyBehavior> Behaviors => pipeline.Behaviors;

		public override int Result
		{
			get
			{
				var method = ((MethodInfo)MethodBase.GetCurrentMethod()).GetBaseDefinition();
				var invocation = new MethodInvocation(this, method);
				var returns = pipeline.Invoke(
					invocation,
					(input, next) => {
						try {
							var returnValue = target != null ?
								target.Result :
								base.Result;

							return input.CreateValueReturn(returnValue);
						} catch (Exception ex) {
							return input.CreateExceptionReturn(ex);
						}
					}
				);

				var exception = returns.Exception;
				if (exception != null)
					throw exception;

				return ((int?)returns.ReturnValue).GetValueOrDefault ();
			}
			set
			{
				var method = ((MethodInfo)MethodBase.GetCurrentMethod()).GetBaseDefinition();
				var invocation = new MethodInvocation(this, method, value);
				var returns = pipeline.Invoke(
					invocation,
					(input, next) => {
						try {
							if (target != null)
								target.Result = value;
							else
								base.Result = value;

							return input.CreateValueReturn(null);
						} catch (Exception ex) {
							return input.CreateExceptionReturn(ex);
						}
					}
				);

				var exception = returns.Exception;
				if (exception != null)
					throw exception;
			}
		}

		public override int this[int x, int y]
		{
			get
			{
				var method = ((MethodInfo)MethodBase.GetCurrentMethod()).GetBaseDefinition();
				var invocation = new MethodInvocation(this, method, x, y);
				var returns = pipeline.Invoke(
					invocation,
					(input, next) => {
						try {
							var returnValue = target != null ?
								target[x, y] :
								base[x, y];

							return input.CreateValueReturn(returnValue, x, y);
						} catch (Exception ex) {
							return input.CreateExceptionReturn(ex);
						}
					}
				);

				var exception = returns.Exception;
				if (exception != null)
					throw exception;

				return ((int?)returns.ReturnValue).GetValueOrDefault ();
			}
			set
			{
				var method = ((MethodInfo)MethodBase.GetCurrentMethod()).GetBaseDefinition();
				var invocation = new MethodInvocation(this, method);
				var returns = pipeline.Invoke(
					invocation,
					(input, next) => {
						try {
							if (target != null)
								target[x, y] = value;
							else
								base[x, y] = value;

							return input.CreateValueReturn(null);
						} catch (Exception ex) {
							return input.CreateExceptionReturn(ex);
						}
					}
				);

				var exception = returns.Exception;
				if (exception != null)
					throw exception;
			}
		}

		public override int Add (int x, int y)
		{
			Func<int, int, int> method = base.Add;
			var invocation = new MethodInvocation(this, method.Method, x, y);
			var returns = pipeline.Invoke(
				invocation,
				(input, next) => {
					try {
						var returnValue = target != null ?
							target.Add(x, y) :
							base.Add(x, y);

						return input.CreateValueReturn(returnValue, x, y);
					} catch (Exception ex) {
						return input.CreateExceptionReturn(ex);
					}
				}
			);

			var exception = returns.Exception;
			if (exception != null)
				throw exception;

			return ((int?)returns.ReturnValue).GetValueOrDefault ();
		}

		delegate bool TryAddInt32Int32Int32Int32 (int seed, ref int x, ref int y, out int z);

		public override bool TryAdd (int seed, ref int x, ref int y, out int z)
		{
			z = default (int);
			var method = this.GetBaseMethod<TryAddInt32Int32Int32Int32>(base.TryAdd);
			var invocation = new MethodInvocation(this, method, seed, x, y, z);
			var returns = pipeline.Invoke(
				invocation,
				(input, next) => {
					try {
						var local_x = (int)invocation.Arguments[0];
						var local_y = (int)invocation.Arguments[1];
						int local_z;
						var returnValue = target == null ?
							base.TryAdd(seed, ref local_x, ref local_y, out local_z) :
							target.TryAdd(seed, ref local_x, ref local_y, out local_z);

						return input.CreateValueReturn(returnValue, seed, local_x, local_y, local_z);
					} catch (Exception ex) {
						return input.CreateExceptionReturn(ex);
					}
				}
			);

			var exception = returns.Exception;
			if (exception != null)
				throw exception;

			x = (int)returns.Outputs[0];
			y = (int)returns.Outputs[1];
			z = (int)returns.Outputs[2];

			return ((bool?)returns.ReturnValue).GetValueOrDefault ();
		}

		public override void TurnOn ()
		{
			Action method = base.TurnOn;
			var invocation = new MethodInvocation(this, method.Method);
			var returns = pipeline.Invoke(
				invocation,
				(input, next) => {
					try {
						if (target != null)
							target.TurnOn();
						else
							base.TurnOn();

						return input.CreateValueReturn(null);
					} catch (Exception ex) {
						return input.CreateExceptionReturn(ex);
					}
				}
			);

			var exception = returns.Exception;
			if (exception != null)
				throw exception;
		}

		void IDisposable.Dispose ()
		{
			Action method = ((IDisposable)this).Dispose;
			var invocation = new MethodInvocation(this, method.Method);
			var returns = pipeline.Invoke(
				invocation,
				(input, next) => {
					try
					{
						if (disposable != null)
							disposable.Dispose();

						return input.CreateValueReturn(null);
					}
					catch (Exception ex)
					{
						return input.CreateExceptionReturn(ex);
					}
				}
			);

			var exception = returns.Exception;
			if (exception != null)
				throw exception;
		}
	}

	public class CalculatorInterfaceProxy : ICalculator, IDisposable, IProxy
	{
		BehaviorPipeline pipeline;
		ICalculator calculator;
		IDisposable disposable;

		public CalculatorInterfaceProxy ()
		{
			pipeline = new BehaviorPipeline ();
		}

		public CalculatorInterfaceProxy (object target)
			: this ()
		{
			calculator = target as ICalculator;
			disposable = target as IDisposable;
		}

		public IList<IProxyBehavior> Behaviors => pipeline.Behaviors;

		public int Result
		{
			get
			{
				throw new NotImplementedException ();
			}

			set
			{
				throw new NotImplementedException ();
			}
		}

		int ICalculator.Add (int x, int y, int z)
		{
			var method = this.GetInterfaceMethod<Func<int, int, int, int>, ICalculator>(((ICalculator)this).Add);
			var invocation = new MethodInvocation(this, method, x, y, z);
			var returns = pipeline.Invoke(
				invocation,
				(input, next) => {
					try
					{
						var returnValue = calculator != null ?
							calculator.Add(x, y, z) :
							default(int);

						return input.CreateValueReturn(returnValue, x, y, z);
					}
					catch (Exception ex)
					{
						return input.CreateExceptionReturn(ex);
					}
				}
			);

			var exception = returns.Exception;
			if (exception != null)
				throw exception;

			return ((int?)returns.ReturnValue).GetValueOrDefault ();
		}

		int ICalculator.Add (int x, int y)
		{
			var method = this.GetInterfaceMethod<Func<int, int, int>, ICalculator>(((ICalculator)this).Add);
			var invocation = new MethodInvocation(this, method, x, y);
			var returns = pipeline.Invoke(
				invocation,
				(input, next) => {
					try {
						var returnValue = calculator != null ?
							calculator.Add(x, y) :
							default(int);

						return input.CreateValueReturn(returnValue, x, y);
					} catch (Exception ex) {
						return input.CreateExceptionReturn(ex);
					}
				}
			);

			var exception = returns.Exception;
			if (exception != null)
				throw exception;

			return ((int?)returns.ReturnValue).GetValueOrDefault ();
		}

		delegate bool TryAddInt32Int32Int32Int32 (int seed, ref int x, ref int y, out int z);

		bool ICalculator.TryAdd (int seed, ref int x, ref int y, out int z)
		{
			z = default (int);
			var method = this.GetInterfaceMethod<TryAddInt32Int32Int32Int32, ICalculator>(((ICalculator)this).TryAdd);
			var invocation = new MethodInvocation(this, method, seed, x, y, z);
			var returns = pipeline.Invoke(
				invocation,
				(input, next) => {
					try {
						var local_x = (int)invocation.Arguments[0];
						var local_y = (int)invocation.Arguments[1];
						int local_z  = default(int);
						var returnValue = calculator != null ?
							calculator.TryAdd(seed, ref local_x, ref local_y, out local_z) :
							default(bool);

						return input.CreateValueReturn(returnValue, seed, local_x, local_y, local_z);
					} catch (Exception ex) {
						return input.CreateExceptionReturn(ex);
					}
				}
			);

			var exception = returns.Exception;
			if (exception != null)
				throw exception;

			x = (int)returns.Outputs[0];
			y = (int)returns.Outputs[1];
			z = (int)returns.Outputs[2];

			return ((bool?)returns.ReturnValue).GetValueOrDefault ();
		}

		void ICalculator.TurnOn ()
		{
			var method = this.GetInterfaceMethod<Action, ICalculator>(((ICalculator)this).TurnOn);
			var invocation = new MethodInvocation(this, method);
			var returns = pipeline.Invoke(
				invocation,
				(input, next) => {
					try {
						if (calculator != null)
							calculator.TurnOn();
						return input.CreateValueReturn(null);
					} catch (Exception ex) {
						return input.CreateExceptionReturn(ex);
					}
				}
			);

			var exception = returns.Exception;
			if (exception != null)
				throw exception;
		}

		void IDisposable.Dispose ()
		{
			var method = this.GetInterfaceMethod<Action, ICalculator>(((ICalculator)this).TurnOn);
			var invocation = new MethodInvocation(this, method);
			var returns = pipeline.Invoke(
				invocation,
				(input, next) => {
					try {
						if (disposable != null)
							disposable.Dispose();
						return input.CreateValueReturn(null);
					} catch (Exception ex) {
						return input.CreateExceptionReturn(ex);
					}
				}
			);

			var exception = returns.Exception;
			if (exception != null)
				throw exception;
		}
	}
}