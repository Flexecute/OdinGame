interface IColdable
{
    /**
     * Applies a cold amount between 0 [none] to 1 [completely stopped] for a given duration
     */ 
    void TakeColdDamage(float slowAmount, float duration);
}