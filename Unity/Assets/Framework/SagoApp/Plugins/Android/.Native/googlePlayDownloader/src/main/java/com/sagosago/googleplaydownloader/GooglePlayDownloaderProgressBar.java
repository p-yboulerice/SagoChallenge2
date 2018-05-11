package com.sagosago.googleplaydownloader;


import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.Rect;
import android.graphics.RectF;
import android.util.AttributeSet;
import android.view.View;

/*
 * Taken from Toca Boca progress bar (TBProgressBar.java)
 */
public class GooglePlayDownloaderProgressBar extends View {

	    private Bitmap backBitmap;
	    private Bitmap middleBitmap;
	    private Bitmap frontBitmap;

	    private Paint bitmapPaint;
        private int mw;
        private int mh;
        private int progress;

        public GooglePlayDownloaderProgressBar(Context context) {
            super(context);
            init();
        }

        public GooglePlayDownloaderProgressBar(Context context, AttributeSet attrs) {
            super(context, attrs);
            init();
        }

	    private void init() {
	    	
	        bitmapPaint = new Paint(Paint.ANTI_ALIAS_FLAG);

	        backBitmap = BitmapFactory.decodeResource(getContext().getResources(), R.drawable.custom_progress_bar_on);
	        middleBitmap = BitmapFactory.decodeResource(getContext().getResources(), R.drawable.custom_progress_bar_off);
	        mw = middleBitmap.getWidth();
	        mh = middleBitmap.getHeight();
	        frontBitmap = BitmapFactory.decodeResource(getContext().getResources(), R.drawable.custom_progress_bar_frame);

	        progress = 0;
	    }

	    public void setProgress(int progress) {
	        this.progress = progress;
	        invalidate();
	    }

	    @Override
	    protected void onMeasure(int widthMeasureSpec, int heightMeasureSpec) {
	        int desiredWidth = mw;
	        int desiredHeight = mh;

	        int widthMode = MeasureSpec.getMode(widthMeasureSpec);
	        int widthSize = MeasureSpec.getSize(widthMeasureSpec);
	        int heightMode = MeasureSpec.getMode(heightMeasureSpec);
	        int heightSize = MeasureSpec.getSize(heightMeasureSpec);

	        int width;
	        int height;

	        if (widthMode == MeasureSpec.EXACTLY) {
	            width = widthSize;
	        } else if (widthMode == MeasureSpec.AT_MOST) {
	            width = Math.min(desiredWidth, widthSize);
	        } else {
	            width = desiredWidth;
	        }

	        if (heightMode == MeasureSpec.EXACTLY) {
	            height = heightSize;
	        } else if (heightMode == MeasureSpec.AT_MOST) {
	            height = Math.min(desiredHeight, heightSize);
	        } else {
	            height = desiredHeight;
	        }

	        setMeasuredDimension(desiredWidth, desiredHeight);
	    }

	    @Override
	    protected void onDraw(Canvas canvas) {
	        super.onDraw(canvas);

	        float p = (float)progress/100.0f;
	        float l = mw*(1.0f-p);
	        float r = mw-l;

	        canvas.drawBitmap(backBitmap, 0.0f, 0.0f, null);

	        Rect src = new Rect(0, 0, (int)l, mh);
	        RectF dst = new RectF(r, 0.0f, r+l, mh);

	        canvas.drawBitmap(middleBitmap, src, dst, null);
	        canvas.drawBitmap(frontBitmap, 0.0f, 0.0f, null);
	    }
	}
