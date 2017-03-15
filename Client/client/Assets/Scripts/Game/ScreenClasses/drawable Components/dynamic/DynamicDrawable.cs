using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

abstract class DynamicDrawable:Drawable
{
    //target position, and scale values
    protected float scale;
    protected Vector2 position;
    protected Vector2 targetPosition;
    protected float targetScale;
    protected float scaleToValue;//dictates how fast the button will scale.
    protected float positionToValue;
    protected float ambient;
    protected float targetAmbient;
    protected float ambientToValue;

    public DynamicDrawable(Vector2 p)
    {
        position = p;
        


        ambient = 1;
        targetPosition = position;
        scale = 1;
        targetScale = 1;
        targetAmbient = 1;
        scaleToValue = .01f;
        ambientToValue = .01f;
    }
    

    public abstract void updateDynamic();

    public override void update()
    {
        updateDynamic();
        updatePosition();
        updateScale();
        updateAmbient();
    }
    public void updateAmbient()
    {
        if (ambient != targetAmbient)
        {
            float ambientVelocity = targetAmbient - ambient;
            if (ambientVelocity < ambientToValue)
            {
                ambient = targetAmbient;
            }
            else
            {
                if (ambientVelocity <= 0)
                {
                    //negative scale velocity
                    ambientVelocity = -ambientVelocity / ambientVelocity;
                }
                else
                    ambientVelocity = ambientVelocity / ambientVelocity;
                ambientVelocity *= ambientToValue;
                ambient += ambientVelocity;
            }
        }
    }

    public void updateScale()
    {
        if (scale != targetScale)
        {
            float scaleVelocity = targetScale - scale;
            if (scaleVelocity < scaleToValue)
            {
                scale = targetScale;
            }
            else
            {
                if (scaleVelocity <= 0)
                {
                    //negative scale velocity
                    scaleVelocity = -scaleVelocity / scaleVelocity;
                }
                else
                    scaleVelocity = scaleVelocity / scaleVelocity;
                scaleVelocity *= scaleToValue;
                scale += scaleVelocity;
            }
        }
    }

    public void updatePosition()
    {
        if (position != targetPosition)
        {
            Vector2 velocity = targetPosition - position;
            if (velocity.magnitude < positionToValue)
            {
                position = targetPosition;
            }
            else
            {
                velocity.Normalize();
                velocity = velocity * positionToValue;
                position += velocity;
            }

        }
    }

    public void setTargetAmbient(float newAmbientTarget)
    {
        targetAmbient = newAmbientTarget;
    }
    public void setAmbientToValue(float newV)
    {
        ambientToValue = newV;
    }
    public bool atTargetAmbient()
    {
        if (ambient == targetAmbient)
            return true;
        return false;
    }

    public void setAmbient(float newA)
    {
        ambient = newA;
    }
    public void setScale(float newS)
    {
        scale = newS;

    }
    public void setPosition(Vector2 newPos)
    {
        position = newPos;
    }

    public void setTargetPosition(Vector2 newTargetPosition)
    {
        targetPosition = newTargetPosition;
    }

    public void setPositionToValue(float newV)
    {
        positionToValue = newV;
    }
    public bool atTargetPosition()
    {
        if (position == targetPosition)
            return true;
        return false;
    }




    public void setTargetScale(float newTargetScale)
    {
        targetScale = newTargetScale;
    }
    public void setToScaleValue(float newV)
    {
        scaleToValue = newV;
    }
    public bool atTargetScale()
    {
        if (targetScale == scale)
            return true;
        return false;
    }
}

