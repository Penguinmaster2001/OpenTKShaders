
#version 330 core

// https://iquilezles.org/articles/distfunctions/

out vec4 FragColor;

uniform vec2 windowSize;



struct Ray
{
    vec3 direction;
    vec3 origin;
};



const int max_steps = 100;



// From the webpage
float dot2(in vec2 v) { return dot(v,v); }
float dot2(in vec3 v) { return dot(v,v); }
float ndot(in vec2 a, in vec2 b) { return a.x*b.x - a.y*b.y; }



float sdSphere(vec3 point, vec3 center, float radius)
{
    return length(point - center) - radius;
}



float sdPlane(vec3 point, vec3 normal, float offset)
{
  return dot(point, normal) - offset;
}



float sdBox(vec3 point, vec3 center, vec3 edgeLengths)
{
  vec3 q = abs(point - center) - edgeLengths;
  return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}



float sdScene(vec3 point)
{
    float sphere = sdSphere(point, vec3(0.0), 1.0);
    float plane = sdPlane(point, vec3(0.0, 1.0, 0.0), -2.0);
    float box = sdBox(point, vec3(-1.0), vec3(1.0));

    return min(min(sphere, plane), box);
}



void main()
{
    vec2 uv = ((2.0 * gl_FragCoord.xy) - windowSize.xy) / min(windowSize.x, windowSize.y);

    Ray ray;
    ray.direction = normalize(vec3(uv, 1));
    ray.origin = vec3(0.0, 0.0, -3.0);


    // Raymarch loop
    float distanceTraveled = 0.0;
    int step;
    for(step = 0; step < max_steps; step++)
    {
        vec3 point = ray.origin + (ray.direction * distanceTraveled);

        float sceneDistance = sdScene(point);

        distanceTraveled += sceneDistance;

        if (sceneDistance < 0.0001) break;

        if (distanceTraveled > 200.0)
        {
            distanceTraveled = 1e10;
            break;
        }
    }
    
    FragColor = vec4(vec3(distanceTraveled * 0.1), 1.0f);
}
