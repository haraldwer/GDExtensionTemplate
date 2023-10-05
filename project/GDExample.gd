extends GDExample


# Called when the node enters the scene tree for the first time.
func _ready():
	set_happy(true)
	set_angry(false)
	print("Am I angry: " + str(is_angry()))
	print("Am I happy: " + str(is_happy()))
	print(GDExample.multiply(5, 7))
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	set_property(get_property() + delta)
	update(delta)
	pass
