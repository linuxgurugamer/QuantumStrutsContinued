// QuantumStrutsContinued
//
// Util.cs
// 
// Continued from QuantumStruts by BoJaN.  Used by permission.
//
// ModuleManager patches © 2014 K3|Chris.  Used by permission.
//
// Copyright © 2014, toadicus
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation and/or other
//    materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using KSP.IO;
using System;
using System.ComponentModel;
using UnityEngine;

namespace QuantumStrut
{
	public static class Util
	{
		static System.Random r = new System.Random();

		public static float Random(float min, float max)
		{
			return (float)((r.NextDouble() * (max - min)) + min);
		}

		public static float Random()
		{
			return (float)r.NextDouble();
		}

		public static Part getTank(Part parent)
		{
			if (parent.Resources.Count != 0)
				return parent;

			if (isValid(parent.parent) && parent.fuelCrossFeed)
				return getTank(parent.parent);

			return null;
		}

		public static Vessel Kerbal
		{
			get
			{
				if (isValid(FlightGlobals.fetch.activeVessel) && FlightGlobals.fetch.activeVessel.isEVA)
					return FlightGlobals.fetch.activeVessel;
				return null;
			}
		}

		public static bool isValid(UnityEngine.Object obj)
		{
			return (obj && obj != null);
		}

		public static void printObj(object obj)
		{
			using (KSP.IO.TextWriter writer = TextWriter.CreateForType<string>("obj.cs"))
			{
				string str = printProperties(obj, TypeDescriptor.GetProperties(obj), 0);
				writer.Write(str);
				MonoBehaviour.print(str);
			}
		}

		static string printProperties(object obj, PropertyDescriptorCollection collection, int I)
		{
			if (I > 2)
				return "";

			string prefix = "";
			string str = "";
			for (int T = 0; T < I; T++)
			{
				prefix += "    ";
			}

			PropertyDescriptor d;
			for (int i = 0; i < collection.Count; i++)
			{
				d = collection[i];

				str += "\n" + prefix + d.Name + " = " + d.GetValue(obj);
			}
			return str;
		}

		public static Part partFromGameObject(GameObject ob)
		{
			GameObject o = ob;

			while (o)
			{
				Part p = Part.FromGO(o);
				if (p && p != null)
				{
					return p;
				}

				if (o.transform.parent)
					o = o.transform.parent.gameObject;
				else
					return null;
			}
			return null;
		}

		public static Part partFromRaycast(RaycastHit hit)
		{
			return partFromGameObject(hit.collider.gameObject);
		}

		public static float GetEnergy(Vessel vessel)
		{
			double energy = 0;
			Part p;
			for (int pIdx = 0; pIdx < vessel.parts.Count; pIdx++)
			{
				p = vessel.parts[pIdx];

				PartResource r;
				for (int rIdx = 0; rIdx < p.Resources.Count; rIdx++)
				{
					r = p.Resources[rIdx];

					if (r.resourceName == "ElectricCharge")
						energy += r.amount;
				}
			}
			return (float)energy;
		}
	}
}
