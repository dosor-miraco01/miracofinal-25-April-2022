using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class WriteCanvas : MonoBehaviour
{
    public static Color Pen_Colour = Color.red;
    public static int Pen_Width = 3;
    public FreeDrawerSettings settings;

    public delegate void Brush_Function(Vector2 world_position);
    // This is the function called when a left click happens
    // Pass in your own custom one to change the brush type
    // Set the default function in the Awake method
    public Brush_Function current_brush;

    public LayerMask Drawing_Layers;

    public bool Reset_Canvas_On_Play = true;
    // The colour the canvas is reset to each time
    public Color Reset_Colour = new Color(0, 0, 0, 0);  // By default, reset the canvas to be transparent

    public Toggle penToggle;
    public GameObject textInputPrefab;
    [SerializeField]
    private bool typingAllow = false;
    public float typingTextSize = 34f;
    
    Image image;
    Sprite drawable_sprite;
    Texture2D drawable_texture;

    Vector2 previous_drag_position;
    Color[] clean_colours_array;
    Color transparent;
    Color32[] cur_colors;
    bool mouse_was_previously_held_down = false;
    bool no_drawing_on_current_drag = false;
    private Color Typeing_Color = Color.red;

    public void SetTypingIsOn(bool isOn)
    {
        typingAllow = isOn;
    }
    //////////////////////////////////////////////////////////////////////////////
    // BRUSH TYPES. Implement your own here


    // When you want to make your own type of brush effects,
    // Copy, paste and rename this function.
    // Go through each step
    public void BrushTemplate(Vector2 world_position)
    {   
        // 1. Change world position to pixel coordinates
        Vector2 pixel_pos = WorldToPixelCoordinates(world_position);

        // 2. Make sure our variable for pixel array is updated in this frame
        cur_colors = drawable_texture.GetPixels32();

        ////////////////////////////////////////////////////////////////
        // FILL IN CODE BELOW HERE

        // Do we care about the user left clicking and dragging?
        // If you don't, simply set the below if statement to be:
        //if (true)

        // If you do care about dragging, use the below if/else structure
        if (previous_drag_position == Vector2.zero)
        {
            // THIS IS THE FIRST CLICK
            // FILL IN WHATEVER YOU WANT TO DO HERE
            // Maybe mark multiple pixels to colour?
            MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
        }
        else
        {
            // THE USER IS DRAGGING
            // Should we do stuff between the previous mouse position and the current one?
            ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
        }
        ////////////////////////////////////////////////////////////////

        // 3. Actually apply the changes we marked earlier
        // Done here to be more efficient
        ApplyMarkedPixelChanges();

        // 4. If dragging, update where we were previously
        previous_drag_position = pixel_pos;
    }

    private Color _selectedColor = Color.red;
    private bool _isPen = true;
    public void SetAsCleaner(bool isOn)
    {
        if (isOn)
        {
            Pen_Colour = Reset_Colour;
            //Pen_Width *= 3;
            Pen_Width = settings.cleanerWidth;
        }
    }

    public void SetAsNormalPen(bool isOn)
    {
        _isPen = isOn;
        if (isOn)
        {
            Pen_Colour = _selectedColor;
            //Pen_Width /= 3;
            Pen_Width = settings.penWidth;
        }
    }

    public void SetColor(ColorVariable color)
    {
        if (penToggle.isOn)
        {
            _selectedColor = color.Value;
            Pen_Colour = color.Value;
        }
        else
        {
            Typeing_Color = color.Value;
        }
    }

    float _xaspectRatio = 0, _yaspectRatio;
    public float XAspectRatio
    {
        get
        {
            if (_xaspectRatio == 0)
            {
                _xaspectRatio = (GUITools.Width / (float)Screen.width);
            }
            return _xaspectRatio;
        }

    }

    public float YAspectRatio
    {
        get
        {
            if (_yaspectRatio == 0)
            {   
                _yaspectRatio = (GUITools.Height / (float)Screen.height);
            }
            return _yaspectRatio;
        }

    }

    // Default brush type. Has width and colour.
    // Pass in a point in WORLD coordinates
    // Changes the surrounding pixels of the world_point to the static pen_colour
    public void PenBrush(Vector2 world_point)
    {   
        Vector2 pixel_pos = new Vector2(Input.mousePosition.x * XAspectRatio, Input.mousePosition.y * YAspectRatio);//WorldToPixelCoordinates(world_point);

        cur_colors = drawable_texture.GetPixels32();

        if (previous_drag_position == Vector2.zero)
        {
            // If this is the first time we've ever dragged on this image, simply colour the pixels at our mouse position
            MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
        }
        else
        {
            // Colour in a line from where we were on the last update call
            ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
        }
        ApplyMarkedPixelChanges();

        //Debug.Log("Dimensions: " + pixelWidth + "," + pixelHeight + ". Units to pixels: " + unitsToPixels + ". Pixel pos: " + pixel_pos);
        previous_drag_position = pixel_pos;
    }

    public Rect GetScreenCoordinates(RectTransform uiElement)
    {
        var worldCorners = new Vector3[4];
        uiElement.GetWorldCorners(worldCorners);
        var result = new Rect(
                      worldCorners[0].x,
                      worldCorners[0].y,
                      worldCorners[2].x - worldCorners[0].x,
                      worldCorners[2].y - worldCorners[0].y);
        return result;
    }

    private bool canCreateTypingField = true;
    public void FinishEditing(TMP_InputField field)
    {
        StartCoroutine(_DoFinishEditing(field));
    }

    IEnumerator _DoFinishEditing(TMP_InputField field)
    {
        //field.interactable = false;
        //yield return new WaitForEndOfFrame();

        textCanvas.gameObject.SetActive(true);
        field.transform.SetParent(textCanvas.transform);
        //yield return new WaitForEndOfFrame();

        textCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        textCanvas.worldCamera = textCamera;

        //yield return new WaitForEndOfFrame();
        
        var mask = field.GetComponentInChildren<RectMask2D>();
        mask.enabled = false;
        
        //yield return new WaitForEndOfFrame();

        textCamera.Render();

        //yield return new WaitForEndOfFrame();
        
        cur_colors = drawable_texture.GetPixels32();

        Texture2D tex = new Texture2D(_textRenderTexture.width, _textRenderTexture.height, TextureFormat.ARGB32, false);
        var currentActive = RenderTexture.active;
        RenderTexture.active = _textRenderTexture;
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = currentActive;
        //yield return new WaitForEndOfFrame();
        //var tms = System.DateTime.Now;
        var colors = tex.GetPixels32();
        for (int i = 0; i < colors.Length; ++i)
        {
            if (colors[i].a == 0) continue;
            cur_colors[i] = Typeing_Color;
        }
        //Debug.Log("time: " + (System.DateTime.Now - tms).TotalSeconds);

        Destroy(tex);
        
        ApplyMarkedPixelChanges();
        textCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        textCanvas.worldCamera = null;
        textCanvas.gameObject.SetActive(false);

        yield return new WaitForEndOfFrame();
        if (field.onEndEdit != null)
        {
            field.onEndEdit.RemoveAllListeners();
        }
        field.DeactivateInputField();
        GameObject.Destroy(field.gameObject, 0.3f);
        Invoke("Set_canCreateTypingField", 0.5f);
    }

    
    void Set_canCreateTypingField()
    {
        canCreateTypingField = true;
    }

    // Helper method used by UI to set what brush the user wants
    // Create a new one for any new brushes you implement
    public void SetPenBrush()
    {
        // PenBrush is the NAME of the method we want to set as our current brush
        current_brush = PenBrush;
    }
    //////////////////////////////////////////////////////////////////////////////






    // This is where the magic happens.
    // Detects when user is left clicking, which then call the appropriate function
    void Update()
    {
        if (_isPen == true)
        {
            Pen_Width = settings.penWidth;
        }
        else
        {
            Pen_Width = settings.cleanerWidth;
        }

        
        // Is the user holding down the left mouse button?
        bool mouse_held_down = Input.GetMouseButton(0);
        if (mouse_held_down && !no_drawing_on_current_drag )
        {
            if (GUITools.IsPointerOverUIObject("FreeWriting")) return;
            // Convert mouse coordinates to world coordinates
            Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (typingAllow)
            {
                if (canCreateTypingField)
                {
                    canCreateTypingField = false;
                       var eventSystem = EventSystem.current;
                    if (!eventSystem.alreadySelecting) eventSystem.SetSelectedGameObject(null);

                    GameObject obj = GameObject.Instantiate(textInputPrefab);
                    obj.name = "Hello";
                    obj.SetActive(true);
                    //penToggle.isOn = true;
                    var tr = obj.GetComponent<Transform>();
                    tr.SetParent(textInputPrefab.transform.parent);
                    tr.position = Input.mousePosition;
                    var tmf = obj.GetComponent<TMP_InputField>();
                    tmf.textComponent.color = Typeing_Color;

                    tmf.Select();
                }
                return;
            }
            // Check if the current mouse position overlaps our image
            //Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
            //if (hit != null && hit.transform != null)
            {
                // We're over the texture we're drawing on!
                // Use whatever function the current brush is
                current_brush(mouse_world_position);
            }

            //else
            //{
            //    // We're not over our destination texture
            //    previous_drag_position = Vector2.zero;
            //    if (!mouse_was_previously_held_down)
            //    {
            //        // This is a new drag where the user is left clicking off the canvas
            //        // Ensure no drawing happens until a new drag is started
            //        no_drawing_on_current_drag = true;
            //    }
            //}
        }
        // Mouse is released
        else if (!mouse_held_down)
        {
            previous_drag_position = Vector2.zero;
            no_drawing_on_current_drag = false;
        }
        mouse_was_previously_held_down = mouse_held_down;
    }



    // Set the colour of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is coloured
    public void ColourBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
    {
        // Get the distance from start to finish
        float distance = Vector2.Distance(start_point, end_point);
        Vector2 direction = (start_point - end_point).normalized;

        Vector2 cur_position = start_point;

        // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
        float lerp_steps = 1 / distance;

        for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
        {
            cur_position = Vector2.Lerp(start_point, end_point, lerp);
            MarkPixelsToColour(cur_position, width, color);
        }
    }

    public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        // Figure out how many pixels we need to colour in each direction (x and y)
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;
        //int extra_radius = Mathf.Min(0, pen_thickness - 2);

        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
            if (x >= (int)drawable_sprite.rect.width || x < 0)
                continue;

            for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
            {
                MarkPixelToChange(x, y, color_of_pen);
            }
        }
    }
    public void MarkPixelToChange(int x, int y, Color color)
    {
        // Need to transform x and y coordinates to flat coordinates of array
        int array_pos = y * (int)drawable_sprite.rect.width + x;

        // Check if this is a valid position
        if (array_pos > cur_colors.Length || array_pos < 0)
            return;

        cur_colors[array_pos] = color;
    }
    public void ApplyMarkedPixelChanges()
    {
        drawable_texture.SetPixels32(cur_colors);
        drawable_texture.Apply();
    }


    // Directly colours pixels. This method is slower than using MarkPixelsToColour then using ApplyMarkedPixelChanges
    // SetPixels32 is far faster than SetPixel
    // Colours both the center pixel, and a number of pixels around the center pixel based on pen_thickness (pen radius)
    public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        // Figure out how many pixels we need to colour in each direction (x and y)
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;
        //int extra_radius = Mathf.Min(0, pen_thickness - 2);

        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
            {
                drawable_texture.SetPixel(x, y, color_of_pen);
            }
        }

        drawable_texture.Apply();
    }


    public Vector2 WorldToPixelCoordinates(Vector2 world_position)
    {
        // Change coordinates to local coordinates of this image
        Vector3 local_pos = transform.InverseTransformPoint(world_position);

        // Change these to coordinates of pixels
        float pixelWidth = drawable_sprite.rect.width;
        float pixelHeight = drawable_sprite.rect.height;
        float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;

        // Need to center our coordinates
        float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
        float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

        // Round current mouse position to nearest pixel
        Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

        return pixel_pos;
    }


    // Changes every pixel to be the reset colour
    public void ResetCanvas()
    {
        if (drawable_texture)
        {
            drawable_texture.SetPixels(clean_colours_array);
            drawable_texture.Apply();
        }
    }

    [SerializeField]
    private Camera textCamera;
    
    [SerializeField]
    private Canvas textCanvas;

    private RenderTexture _textRenderTexture;
    void Awake()
    {   
        current_brush = PenBrush;

        image = this.GetComponent<Image>();
        drawable_sprite = image.sprite;
        drawable_texture = drawable_sprite.texture;

        // Initialize clean pixels to use
        clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
        for (int x = 0; x < clean_colours_array.Length; x++)
            clean_colours_array[x] = Reset_Colour;

        // Should we reset our canvas image when we hit play in the editor?
        if (Reset_Canvas_On_Play)
            ResetCanvas();

        if (_textRenderTexture == null)
        {
            _textRenderTexture = new RenderTexture(drawable_texture.width, drawable_texture.height, 16, RenderTextureFormat.ARGB32);
            _textRenderTexture.antiAliasing = 2;
            _textRenderTexture.Create();

            textCamera.targetTexture = _textRenderTexture;

            textCamera.gameObject.SetActive(true);
            textCanvas.gameObject.SetActive(false);
        }
    }
}
