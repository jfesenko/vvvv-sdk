#region licence/info

//////project name
//2d gui nodes

//////description
//nodes to build 2d guis in a EX9 renderer

//////licence
//GNU Lesser General Public License (LGPL)
//english: http://www.gnu.org/licenses/lgpl.html
//german: http://www.gnu.de/lgpl-ger.html

//////language/ide
//C# sharpdevelop 

//////dependencies
//VVVV.PluginInterfaces.V1;
//VVVV.Utils.VMath;

//////initial author
//tonfilm

#endregion licence/info

//use what you need
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using VVVV.PluginInterfaces.V1;
using VVVV.Utils.VMath;
using VVVV.Utils.VColor;
using VVVV.Nodes.src.Controllers;


namespace VVVV.Nodes
{
	public class MTSliderXYNode: MTBasicGui2dNode<SliderXYGroup,SliderXY>, IPlugin
	{
		#region field declaration

		//additional slider size pin
		private IValueIn FSizeSliderIn;
		private IValueIn FSliderSpeedIn;
        private ITransformOut FPinOutSliderTransform;
		
		#endregion field declaration
		
		#region constructor/destructor
    	
        public MTSliderXYNode()
        {
			//the nodes constructor
			//nothing to declare for this node
		}
        
        #endregion constructor/destructor
		
		#region node name and infos

		//provide node infos
		public static IPluginInfo PluginInfo
		{
			get
			{
				//fill out nodes info
				IPluginInfo Info = new PluginInfo();
				Info.Name = "MTSliderXY";
				Info.Category = "2d GUI";
				Info.Version = "";
				Info.Help = "A spread of xy-slider groups";
				Info.Tags = "EX9, DX9, transform, interaction, mouse, slider, fader";
				Info.Author = "tonfilm";
				Info.Bugs = "";
				Info.Credits = "";
				Info.Warnings = "";
				
				//leave below as is
				System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
				System.Diagnostics.StackFrame sf = st.GetFrame(0);
				System.Reflection.MethodBase method = sf.GetMethod();
				Info.Namespace = method.DeclaringType.Namespace;
				Info.Class = method.DeclaringType.Name;
				return Info;
				//leave above as is
			}
		}
		
		#endregion node name and infos
		
		#region pin creation
		
		//this method is called by vvvv when the node is created
		public override void SetPluginHost(IPluginHost Host)
		{
			
			//assign host
			FHost = Host;

			//create inputs:
			
			//transform
			FHost.CreateTransformInput("Transform In", TSliceMode.Dynamic, TPinVisibility.True, out FTransformIn);
            //FHost.CreateValueInput("Transform In", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out FTransformIn);
			
			//value
			FHost.CreateValueInput("Value Input ", 2, null, TSliceMode.Dynamic, TPinVisibility.True, out FValueIn);
			FValueIn.SetSubType2D(0, 1, 0.01, 0, 0, false, false, false);
			
			FHost.CreateValueInput("Set Value", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out FSetValueIn);
			FSetValueIn.SetSubType(0, 1, 1, 0, true, false, false);
			
			//counts
			FHost.CreateValueInput("Count X", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out FCountXIn);
			FCountXIn.SetSubType(1, double.MaxValue, 1, 1, false, false, true);
			
			FHost.CreateValueInput("Count Y", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out FCountYIn);
			FCountYIn.SetSubType(1, double.MaxValue, 1, 1, false, false, true);
			
			//size
			FHost.CreateValueInput("Size X", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out FSizeXIn);
			FSizeXIn.SetSubType(double.MinValue, double.MaxValue, 0.01, 0.9, false, false, false);
			
			FHost.CreateValueInput("Size Y", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out FSizeYIn);
			FSizeYIn.SetSubType(double.MinValue, double.MaxValue, 0.01, 0.9, false, false, false);
			
			//mouse
            this.FHost.CreateValueInput("Touch Id", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out this.FPinInTouchId);
            this.FPinInTouchId.SetSubType(double.MinValue, double.MaxValue, 1, 0, false, false, true);

            this.FHost.CreateValueInput("Touch Position", 2, null , TSliceMode.Dynamic, TPinVisibility.True, out this.FPinInTouchPos);
            this.FPinInTouchPos.SetSubType2D(double.MinValue, double.MaxValue, 0.01, 0, 0, false, false, false);

            this.FHost.CreateValueInput("Is new", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out this.FPinInIsNew);
            this.FPinInIsNew.SetSubType(0, 1, 1, 0, false, true, false);
			
			//color
			
			FHost.CreateValueInput("Size Slider", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out FSizeSliderIn);
			FSizeSliderIn.SetSubType(double.MinValue, double.MaxValue, 0.01, 0.02, false, false, false);
			
			FHost.CreateValueInput("Slider Speed", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out FSliderSpeedIn);
			FSliderSpeedIn.SetSubType(0, double.MaxValue, 0.01, 1, false, false, false);
   
        
			//create outputs
			FHost.CreateTransformOutput("Transform Out", TSliceMode.Dynamic, TPinVisibility.True, out FTransformOut);

            FHost.CreateTransformOutput("Slider Transform Out", TSliceMode.Dynamic, TPinVisibility.True, out FPinOutSliderTransform);
			
			
			FHost.CreateValueOutput("Value Output ", 2, null, TSliceMode.Dynamic, TPinVisibility.True, out FValueOut);
			FValueOut.SetSubType2D(0, 1, 0.01, 0, 0, false, false, false);
					
			FHost.CreateValueOutput("Hit", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out FHitOut);
			FHitOut.SetSubType(0, 1, 1, 0, true, false, false);
			
			
			FHost.CreateValueOutput("Spread Counts", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out FSpreadCountsOut);
			FSpreadCountsOut.SetSubType(0, double.MaxValue, 0.01, 0, false, false, true);
			
			
			FControllerGroups = new List<SliderXYGroup>();
			
		}
		
		#endregion pin creation
		
		#region mainloop
		
		public override void Evaluate(int SpreadMax)
		{
			
			//calc input spreadcount
			int inputSpreadCount = GetSpreadMax();
			
			//create or delete button groups
			int diff = inputSpreadCount - FControllerGroups.Count;
			if (diff > 0)
			{
				for (int i=0; i<diff; i++)
				{
					FControllerGroups.Add(new SliderXYGroup());
				}
			}
			else if (diff < 0)
			{
				for (int i=0; i< -diff; i++)
				{
					FControllerGroups.RemoveAt(FControllerGroups.Count-1-i);
				}
			}
			
			//update parameters
			int slice;
			if (   FCountXIn.PinIsChanged
			    || FCountYIn.PinIsChanged
			    || FSizeXIn.PinIsChanged
			    || FSizeYIn.PinIsChanged
			    || FSizeSliderIn.PinIsChanged
			    || FTransformIn.PinIsChanged
			    || FSliderSpeedIn.PinIsChanged)
			{
				
				for (slice = 0; slice < inputSpreadCount; slice++)
				{
					SliderXYGroup group = (SliderXYGroup) FControllerGroups[slice];
					
					Matrix4x4 trans;
					Vector2D count, size;
					double sizeSlider, sliderSpeed;
					
					FTransformIn.GetMatrix(slice, out trans);
					FCountXIn.GetValue(slice, out count.x);
					FCountYIn.GetValue(slice, out count.y);
					FSizeXIn.GetValue(slice, out size.x);
					FSizeYIn.GetValue(slice, out size.y);
					FSizeSliderIn.GetValue(slice, out sizeSlider);
					FSliderSpeedIn.GetValue(slice, out sliderSpeed);

					group.UpdateTransform(trans, count, size, sizeSlider, sliderSpeed);
					
				}
			}
			
			//get spread counts
			int outcount = 0;
			FSpreadCountsOut.SliceCount = inputSpreadCount;
			
			for (slice = 0; slice < inputSpreadCount; slice++)
			{
				SliderXYGroup group = (SliderXYGroup) FControllerGroups[slice];
				
				outcount += group.FControllers.Length;
				FSpreadCountsOut.SetValue(slice, group.FControllers.Length);
				
			}

            this.UpdateTouches(inputSpreadCount);

            #region Set Value
            slice = 0;
			if (   FValueIn.PinIsChanged
			    || FSetValueIn.PinIsChanged)
			{
				
				for (int i = 0; i < inputSpreadCount; i++)
				{
					SliderXYGroup group = FControllerGroups[i];
					int pcount = group.FControllers.Length;
					
					for (int j = 0; j < pcount; j++)
					{
						
						Vector2D val;
						
						FSetValueIn.GetValue(slice, out val.x);

                        if (val.x >= 0.5 || this.FFirstframe)
						{
							//update value
							FValueIn.GetValue2D(slice, out val.x, out val.y);
							group.UpdateValue(group.FControllers[j], val);
						}
					
						slice++;
					}
				}
            }
            #endregion

            //write output to pins
			FValueOut.SliceCount = outcount;
			FTransformOut.SliceCount = outcount;
            FPinOutSliderTransform.SliceCount = outcount;
			FHitOut.SliceCount = outcount;
			
			slice = 0;
			for (int i = 0; i < inputSpreadCount; i++)
			{
				SliderXYGroup group = FControllerGroups[i];
				int pcount = group.FControllers.Length;
				
				for (int j = 0; j < pcount; j++)
				{
					SliderXY s = group.FControllers[j];
					
					FTransformOut.SetMatrix(slice, s.Transform);
					FPinOutSliderTransform.SetMatrix(slice, s.SliderTransform);
					FValueOut.SetValue2D(slice, s.Value.x, s.Value.y);
					FHitOut.SetValue(slice, s.Hit ? 1 : 0);
								
					slice++;
				}
			}

			//end of frame
			FFirstframe = false;
		}
		
		//calc how many groups are required
		private int GetSpreadMax()
		{		
			int max = 0;	
			max = Math.Max(max, FCountXIn.SliceCount);
			max = Math.Max(max, FCountYIn.SliceCount);
			max = Math.Max(max, FSizeXIn.SliceCount);
			max = Math.Max(max, FSizeYIn.SliceCount);		
			max = Math.Max(max, FSizeSliderIn.SliceCount);
			max = Math.Max(max, FSliderSpeedIn.SliceCount);	
			max = Math.Max(max, FTransformIn.SliceCount);
            return max;			
		}
		
		#endregion mainloop


    }
	


}



