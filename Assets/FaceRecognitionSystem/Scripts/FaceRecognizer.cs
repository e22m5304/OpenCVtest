using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FaceRecognitionSystem;
using System.Threading;
using System;
using System.Text;

[Serializable]
public class Face {
	public string Name;
	public Color BorderColor = Color.green;
	public Texture2D Image;
}

public class FaceRecognizer : MonoBehaviour {
	public List<Face> SearchableFaces = new List<Face>( );
	public List<FaceDescription> FoundFaces = new List<FaceDescription>( );
	[SerializeField]
	GameObject right1;
	[SerializeField]
	GameObject right; 
	[SerializeField]
	GameObject left1;
	[SerializeField]
	GameObject left;
	public RawImage View = null;
	public float SearchThreshold = 0.5f;
	public bool DrawKeypoints = false;
	public bool OnlyDetection = false;
    public static string koji2 = "koji";
	public static string kaiji = "kaiji";
	public static string saitou = "saitou";
	public static string mask = "mask";
	public static string sou = "sou";
	

	public Text Log;

	public void OnImageProviderReady( IImageProvider imgProvider ) {
		_frs.Init( );
		_imageProvider = imgProvider;
		if ( ( _dt == null ) || ( _dt.width != _imageProvider.Width ) || ( _dt.height != _imageProvider.Height ) ) {
			_dt = new Texture2D( _imageProvider.Width, _imageProvider.Height );
		}
		View.texture = _dt;
		_targetFacesDescs.Clear( );

		foreach ( var f in SearchableFaces ) {
			var facesLst = _frs.FindFaces( f.Image.GetPixels32( ), f.Image.width, f.Image.height );		
			for ( int i = 0; i < facesLst.Count; ++i ) {
				facesLst [ i ].Name = f.Name;
				facesLst [ i ].BorderColor = f.BorderColor;
				_targetFacesDescs.Add( facesLst [ i ] );
            }
		}
		if ( Log != null )
			Log.text = string.Empty;

	}

	private void Awake( ) {
		_frs = new FRS( );
		_frs.OnlyDetection = OnlyDetection;
		_frs.log += this.appendLog;
		_frs.SearchThreshold = SearchThreshold;
	}

	private void Update( ) {
		if ( ( _imageProvider != null ) && ( _imageProvider.ImgData != null ) ) {
			if ( _imageProvider.ImgData.Length > 0 ) {
				if ( !_frs.IsBusy( ) ) {
					FoundFaces = _frs.FindFaces( _imageProvider.ImgData, _imageProvider.Width, _imageProvider.Height );
					if ( FoundFaces.Count > 0 ) {
						if ( _targetFacesDescs.Count > 0 ) {
							FoundFaces = _frs.FindEqualFaces( _targetFacesDescs, FoundFaces );
							printResults( FoundFaces );
						}
					}
				}
				drawResults( _imageProvider.ImgData, FoundFaces );
				
			}
		}
	}

	private void printResults( List<FaceDescription> results ) {
		if ( Log == null )
			return;
		Log.text = string.Empty;
		foreach ( var f in results ) {
			if ( f.Name != string.Empty ) {
				var sb = new StringBuilder( );
				sb.Append( "<color=#" );
				sb.Append( ColorUtility.ToHtmlStringRGB( f.BorderColor ) );
				sb.Append( ">" );
				sb.Append( f.Name );
				sb.Append( "</color>" );
				appendLog( sb.ToString( ) );

				//Debug.Log("a");
				//Debug.Log($"Found face: {f.Name}");
				
			}
		}
	}

	private void drawResults(Color32[] rawImg, List<FaceDescription> results)
	{
		const int THICKNESS = 1;
		if (results != null)
		{
			foreach (var r in results)
			{
				Color BORDER_COLOR = r.BorderColor;
				if (DrawKeypoints)
				{
					markPixelsToColour(rawImg, r.LeftEye, THICKNESS, Color.white);
					markPixelsToColour(rawImg, r.RightEye, THICKNESS, Color.white);
					markPixelsToColour(rawImg, r.Nose, THICKNESS, Color.white);
					markPixelsToColour(rawImg, r.Mouth, THICKNESS, Color.white);
					markPixelsToColour(rawImg, r.LeftEar, THICKNESS, Color.white);
					markPixelsToColour(rawImg, r.RightEar, THICKNESS, Color.white);

					
				}
				for (var x = r.Rect.x; x < (r.Rect.x + r.Rect.width); ++x)
				{
					markPixelsToColour(rawImg, new Vector2(x, r.Rect.y), THICKNESS, BORDER_COLOR);
					markPixelsToColour(rawImg, new Vector2(x, r.Rect.y - r.Rect.height), THICKNESS, BORDER_COLOR);
				}
				for (var y = r.Rect.y; y > (r.Rect.y - r.Rect.height); --y)
				{
					markPixelsToColour(rawImg, new Vector2(r.Rect.x, y), THICKNESS, BORDER_COLOR);
					markPixelsToColour(rawImg, new Vector2(r.Rect.x + r.Rect.width, y), THICKNESS, BORDER_COLOR);
				}
				//Debug.Log("a");
				foreach (var f in results)
				{
					if (f.Name != string.Empty)
					{
						var sb = new StringBuilder();
						sb.Append("<color=#");
						sb.Append(ColorUtility.ToHtmlStringRGB(f.BorderColor));
						sb.Append(">");
						sb.Append(f.Name);
						sb.Append("</color>");
						appendLog(sb.ToString());
						//
						//Debug.Log($"Found face: {f.Name}");
						
						if (f.Name == kaiji)
                        {
							right.SetActive(true);
							right1.SetActive(false);
							left.SetActive(true);
							left1.SetActive(false);
							//Debug.Log("a");
						}
						else if (f.Name == koji2)
						{
							right1.SetActive(false);
							right.SetActive(true);
							left1.SetActive(false);
							left.SetActive(true);
						}
						else if(f.Name == mask) {
							right1.SetActive(true);
							right.SetActive(false);
							left1.SetActive(true);
							left.SetActive(false);
						}
						if (f.Name == sou)
						{
							right1.SetActive(true);
							right.SetActive(false);
							left1.SetActive(true);
							left.SetActive(false);
						}
					}
				}

				//DrawRectangle(rawImg, r.Rect, THICKNESS, BORDER_COLOR);
			}
		}

		_dt.SetPixels32(rawImg);
		_dt.Apply();
	}
	private void DrawRectangle(Color32[] rawImg, Rect rect, int thickness, Color color)
	{
		int x = (int)rect.x;
		int y = (int)rect.y;
		int width = (int)rect.width;
		int height = (int)rect.height;

		for (int i = x; i < x + width; i++)
		{
			for (int j = y - height; j < y; j++)
			{
				markPixelToChange(rawImg, i, j, color);
			}
		}

		for (int i = x; i < x + width; i++)
		{
			for (int j = y; j > y - height; j--)
			{
				markPixelToChange(rawImg, i, j, color);
			}
		}
	}
	private void markPixelsToColour( Color32 [ ] rawImg, Vector2 center_pixel, int pen_thickness, Color color_of_pen ) {
		int center_x = (int)center_pixel.x;
		int center_y = (int)center_pixel.y;
		for ( int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++ ) {
			if ( x >= ( int )_imageProvider.Width || x < 0 )
				continue;
			for ( int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++ ) {
				markPixelToChange( rawImg, x, y, color_of_pen );
			}
		}
	}

	private void markPixelToChange( Color32 [ ] rawImg, int x, int y, Color color ) {
		int array_pos = y * (int)_imageProvider.Width + x;
		if ( array_pos > rawImg.Length || array_pos < 0 )
			return;
		rawImg [ array_pos ] = color;
	}

	private void OnDestroy( ) {
		_frs.Dispose( );
	}

	private void appendLog( string mess ) {
		if ( Log != null )
			Log.text = mess + "\n" + Log.text;
	}

	IImageProvider _imageProvider = null;
	private Texture2D _dt = null;
	private List<FaceDescription> _targetFacesDescs = new List<FaceDescription>( );
	private FRS _frs = null;
    
}
