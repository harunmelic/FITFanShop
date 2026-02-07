import { Component, Input, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-rating-input',
  standalone: false,
  templateUrl: './rating-input.component.html',
  styleUrl: './rating-input.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RatingInputComponent),
      multi: true
    }
  ]
})
export class RatingInputComponent implements ControlValueAccessor {
  @Input() required: boolean = false;
  
  value: number = 0;
  hoveredStar: number = 0;
  disabled: boolean = false;

  private onChange = (value: number) => {};
  private onTouched = () => {};

  get stars(): number[] {
    return [1, 2, 3, 4, 5];
  }

  getStarClass(star: number): string {
    let className = 'star';
    
    if (this.disabled) {
      className += ' disabled';
    }

    const effectiveRating = this.hoveredStar || this.value;
    
    if (effectiveRating >= star) {
      className += ' filled';
    } else {
      className += ' empty';
    }

    return className;
  }

  onStarClick(star: number): void {
    if (this.disabled) return;
    
    this.value = star;
    this.onChange(this.value);
    this.onTouched();
  }

  onStarHover(star: number): void {
    if (this.disabled) return;
    this.hoveredStar = star;
  }

  onMouseLeave(): void {
    this.hoveredStar = 0;
  }

  // ControlValueAccessor implementation
  writeValue(value: number): void {
    this.value = value || 0;
  }

  registerOnChange(fn: (value: number) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}