// QuantumStrutsContinued
//
// QuantumStrut.cs
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

using KSP;
using System;
//using ToadicusTools;
using UnityEngine;

namespace QuantumStrut
{
	public class QuantumStrut : PartModule
	{
		public static Material LaserMaterial;
		Strut strut = null;
		GameObject lineObj;
		LineRenderer lr;
		int I = 0;

		#region Fields

		[KSPField(isPersistant = true)]
		public bool IsEnabled = true;
		[KSPField(isPersistant = false)]
		public float PowerConsumption = 0;
		[KSPField(isPersistant = false)]
		public string TransformName = "";
		[KSPField(isPersistant = false)]
		public Vector3 Start = new Vector3(0, 0, 0);
		[KSPField(isPersistant = false)]
		public Vector3 Dir = new Vector3(0, 1, 0);
		[KSPField(isPersistant = false)]
		public string Material = "Particles/Additive";
		[KSPField(isPersistant = false)]
		public Vector3 StartColor = Vector3.zero;
		[KSPField(isPersistant = false)]
		public Vector3 EndColor = Vector3.zero;
		[KSPField(isPersistant = false)]
		public float StartSize = 0.03f;
		[KSPField(isPersistant = false)]
		public float EndSize = 0.015f;

		[KSPField(
			isPersistant = true, guiActiveEditor = true,
			guiName = "Max. Strut Length", guiUnits = "m", guiFormat = "F0"
		)]
		[UI_FloatRange(minValue = 2f, maxValue = 50f, stepIncrement = 2f)]
		public float MaxStrutLength = 10f;

		#endregion

		#region Actions

		[KSPAction("Toggle")]
		public void ToggleStrut(KSPActionParam param)
		{
			IsEnabled = !IsEnabled;
			CheckHit();
		}

		[KSPAction("Activate")]
		public void ActivateStrut(KSPActionParam param)
		{
			this.ActivateStrut();
		}

		[KSPAction("Deactivate")]
		public void DeactivateStrut(KSPActionParam param)
		{
			this.DeactivateStrut();
		}

		#endregion

		#region Events

		[KSPEvent(guiActive = true, guiName = "Activate", active = true, guiActiveUnfocused = true, unfocusedRange = 2f)]
		public void ActivateStrut()
		{
			IsEnabled = true;
			CheckHit();
			this.Events["ActivateStrut"].guiActiveEditor = false;
			this.Events["DeactivateStrut"].guiActiveEditor = true;
		}

		[KSPEvent(guiActive = true, guiName = "Deactivate", active = false, guiActiveUnfocused = true, unfocusedRange = 2f)]
		public void DeactivateStrut()
		{
			IsEnabled = false;
			CheckHit();
			this.Events["ActivateStrut"].guiActiveEditor = true;
			this.Events["DeactivateStrut"].guiActiveEditor = false;
		}

		#endregion

		public Material material = null;
		public Color startColor = Color.white;
		public Color endColor = Color.white;

		public Color Vector3toColor(Vector3 vec)
		{
			return new Color(vec.x / 255, vec.y / 255, vec.z / 255);
		}

		Transform getTransform()
		{
			if (TransformName == "")
				return part.transform;
			else
				return part.FindModelTransform(TransformName);
		}

		public void print(object body, params object[] args)
		{
			string final = body.ToString();
			for (int I = 0; I < args.Length; I++)
			{
				final = final.Replace("{" + I + "}", args[I].ToString());
			}
			MonoBehaviour.print("[AutoStrut] " + final);
		}

		public override void OnLoad(ConfigNode node)
		{
			base.OnLoad(node);
		}

		public override void OnActive()
		{
			InitLaser();

			base.OnActive();
		}

		public override void OnInactive()
		{
			DestroyLaser();

			base.OnInactive();
		}

		public override string GetInfo()
		{
			return "Requires:\n- ElectricCharge (" + PowerConsumption + "/s.)\n\n Costs 5 to create strut.";
		}

		public override void OnStart(PartModule.StartState state)
		{
			try
			{
				print("Material: {0}", Material);
				material = new Material(Shader.Find(Material.Trim()));
			}
			catch
			{
				material = null;
			}

			base.stagingEnabled = false;

			startColor = Vector3toColor(StartColor);
			endColor = Vector3toColor(EndColor);

			if (!Util.isValid(LaserMaterial))
				LaserMaterial = new Material(Shader.Find("Particles/Additive"));
			
			switch (state)
			{
				case StartState.Editor:
					InitLaser();
					break;
				case StartState.Docked:
					CheckHit();
					DestroyLaser();
					break;
				default:
					DestroyLaser();
					break;
			}

			base.OnStart(state);
		}

		public override bool IsStageable()
		{
			return false;
		}

		public void Update()
		{
			if (strut != null && !strut.isDestroyed)
			{
				if (PowerConsumption == 0 || (Util.GetEnergy(part.vessel) > PowerConsumption * TimeWarp.fixedDeltaTime && part.RequestResource(
					"ElectricCharge",
					PowerConsumption * TimeWarp.fixedDeltaTime
				) > 0))
				{
					strut.Update();
				}
				else
				{
					strut.Destroy();
				}
			}
		}

		public void FixedUpdate()
		{
			Events["ActivateStrut"].guiActiveEditor = Events["ActivateStrut"].active = !IsEnabled;
			Events["DeactivateStrut"].guiActiveEditor = Events["DeactivateStrut"].active = IsEnabled;

			if (IsEnabled)
			{
				I = I + 1 % 255;

				if (strut == null || strut.isDestroyed)
				{
					if ((I % 10) == 0)
					{
						CheckHit();
					}
				}
			}
			else
			{
				if (strut != null)
				{
					strut.Destroy();
					strut = null;
				}
			}

			base.OnUpdate();
		}

		public void OnGUI()
		{
			if (HighLogic.LoadedSceneIsEditor && Util.isValid(part))
			{
				this.DrawBuildOverlay();
			}
			else
			{
				DestroyLaser();
			}
		}

		public void OnDestroy()
		{
			DestroyLaser();
		}

		void CheckHit()
		{
			if (HighLogic.LoadedSceneIsEditor)
			{
				Logging.PostDebugMessage(this, "Checking bailing out: in the editor!");
				return;
			}

			if (!isEnabled)
			{
				Logging.PostDebugMessage(this, "Destroying strut.");

				strut.Destroy();
				strut = null;
				return;
			}

			Logging.PostDebugMessage(this, "Checking for ray hit.");

			Logging.PostDebugMessage(this, "Enabled, continuing.");

			if (strut == null || strut.isDestroyed)
			{
				Logging.PostDebugMessage(this, "strut is {0}", strut == null ? "null" : strut.isDestroyed.ToString());

				Vector3 dir = getTransform().TransformDirection(Dir);
				Vector3 start = getTransform().TransformPoint(Start);

				Logging.PostDebugMessage(this, "Got transforms.  Checking for raycast hit.");

				UnityEngine.RaycastHit info = new RaycastHit();
				bool hit = Physics.Raycast(new UnityEngine.Ray(start + (dir * 0.05f), dir), out info, MaxStrutLength);

				if (hit)
				{
					Logging.PostDebugMessage(this, "Found raycast hit.  Fetching target part.");

					Part targetPart = Util.partFromRaycast(info);

					Logging.PostDebugMessage(this,
						"Found target part {0} on {1}.",
						targetPart.partName,
						targetPart.vessel == null ? "null vessel" : targetPart.vessel.vesselName
					);

					if (
						targetPart && vessel.parts.Contains(targetPart) &&
						Util.GetEnergy(part.vessel) > 5 * TimeWarp.fixedDeltaTime
					)
					{
						Logging.PostDebugMessage(this, "Target part is in our vessel and we have the energy to continue.");

						strut = new Strut(
							part,
							targetPart,
							targetPart.transform.InverseTransformPoint(info.point),
							getTransform()
						);

						Logging.PostDebugMessage(this, "Built a new strut, setting material, colors, and sizes.");

						strut.Material = material;
						strut.StartColor = startColor;
						strut.EndColor = endColor;
						strut.StartSize = StartSize;
						strut.EndSize = EndSize;

						Logging.PostDebugMessage(this, "Strut all done!");
					}
				}
			}
		}

		void InitLaser()
		{
			if (!Util.isValid(lr))
			{
				lineObj = new GameObject();

				lr = lineObj.AddComponent<LineRenderer>();
				lr.useWorldSpace = false;

				lr.material = material;
				lr.SetColors(startColor, endColor);
				lr.SetWidth(StartSize, EndSize);

				lr.SetVertexCount(2);
				lr.SetPosition(0, Vector3.zero);
				lr.SetPosition(1, Vector3.zero);
				lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				lr.receiveShadows = true;
			}
		}

		void DestroyLaser()
		{
			if (Util.isValid(lr))
				LineRenderer.DestroyImmediate(lr);

			if (Util.isValid(lineObj))
				GameObject.DestroyImmediate(lineObj);
		}

		public void DrawBuildOverlay()
		{
			if (Util.isValid(lr))
			{
				Vector3 dir = getTransform().TransformDirection(Dir);
				Vector3 start = getTransform().TransformPoint(Start);

				UnityEngine.RaycastHit info = new RaycastHit();
				bool hit = Physics.Raycast(new UnityEngine.Ray(start + (dir * 0.05f), dir), out info, MaxStrutLength);
				if (hit && IsEnabled)
				{
					if (Util.isValid(material))
						lr.material = material;

					lr.SetColors(startColor, endColor);
					lr.SetWidth(StartSize, EndSize);

					lr.SetPosition(0, start);
					lr.SetPosition(1, info.point);
				}
				else
				{
					lr.material = LaserMaterial;
					lr.SetColors(Color.red, Color.red);
					lr.SetWidth(0.01f, 0.01f);

					lr.SetPosition(0, start);
					lr.SetPosition(1, start + (dir * MaxStrutLength));
				}
			}
		}
	}
}